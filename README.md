# Weather Service API

ASP.NET Core Web API that proxies weather data from the [SMHI open data API](https://opendata.smhi.se/).

## Running

```
dotnet run
```

Swagger UI: http://localhost:5000/swagger/index.html

## Endpoints

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/weather/temperature` | Average air temperature across all SMHI stations for the latest hour |
| GET | `/api/weather/rainfall/{city}` | Total rainfall in mm for a city over the recent period *(not yet implemented)* |

## Testing

```
dotnet test
```

Unit tests cover `WeatherService` in isolation using a fake `ISmhiClient` (no HTTP calls):

| Test | What it verifies |
|------|-----------------|
| Returns correct average | Average of valid station readings |
| Uses first reading per station | Only the first value per station is used |
| Ignores empty/non-numeric | Stations with missing or non-numeric values are skipped |
| Throws on no usable readings | `SmhiUnavailableException` when all stations are invalid |
| Throws on empty station list | `SmhiUnavailableException` when SMHI returns no stations |
| Parses decimals under comma-decimal culture | `InvariantCulture` used — Swedish locale (`sv-SE`) doesn't break parsing |

## Configuration

`appsettings.json`:

```json
"WeatherApi": {
  "SmhiBaseUrl": "https://opendata-download-metobs.smhi.se/api/version/latest"
}
```
