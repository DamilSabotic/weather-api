namespace CodeAssignmentTemplate.Exceptions;

public sealed class CityNotFoundException : Exception
{
    public string City { get; }

    public CityNotFoundException(string city, string message) : base(message) => City = city;
}
