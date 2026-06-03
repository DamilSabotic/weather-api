using CodeAssignmentTemplate.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CodeAssignmentTemplate.Tests.Infrastructure;

public class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task TryHandleAsync_ClientAborted_WritesNoResponse()
    {
        var problemDetails = new RecordingProblemDetailsService();
        var handler = new GlobalExceptionHandler(problemDetails, NullLogger<GlobalExceptionHandler>.Instance);

        using var aborted = new CancellationTokenSource();
        aborted.Cancel();
        var context = new DefaultHttpContext { RequestAborted = aborted.Token };

        var handled = await handler.TryHandleAsync(
            context, new OperationCanceledException(), CancellationToken.None);

        Assert.False(handled);
        Assert.False(problemDetails.WriteCalled);
    }

    [Fact]
    public async Task TryHandleAsync_UpstreamTimeout_MapsToBadGateway()
    {
        var problemDetails = new RecordingProblemDetailsService();
        var handler = new GlobalExceptionHandler(problemDetails, NullLogger<GlobalExceptionHandler>.Instance);

        // RequestAborted is not cancelled, so the cancellation is an upstream timeout.
        var context = new DefaultHttpContext();

        var handled = await handler.TryHandleAsync(
            context, new OperationCanceledException(), CancellationToken.None);

        Assert.True(handled);
        Assert.True(problemDetails.WriteCalled);
        Assert.Equal(StatusCodes.Status502BadGateway, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_HttpRequestException_MapsToBadGateway()
    {
        var problemDetails = new RecordingProblemDetailsService();
        var handler = new GlobalExceptionHandler(problemDetails, NullLogger<GlobalExceptionHandler>.Instance);

        var context = new DefaultHttpContext();

        var handled = await handler.TryHandleAsync(
            context, new HttpRequestException("boom"), CancellationToken.None);

        Assert.True(handled);
        Assert.True(problemDetails.WriteCalled);
        Assert.Equal(StatusCodes.Status502BadGateway, context.Response.StatusCode);
    }

    private sealed class RecordingProblemDetailsService : IProblemDetailsService
    {
        public bool WriteCalled { get; private set; }

        public ValueTask WriteAsync(ProblemDetailsContext context)
        {
            WriteCalled = true;
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> TryWriteAsync(ProblemDetailsContext context)
        {
            WriteCalled = true;
            return ValueTask.FromResult(true);
        }
    }
}
