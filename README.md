# Weather Service API

ASP.NET Core Web API that proxies weather data from the [SMHI open data API](https://opendata.smhi.se/).

## Running

```
dotnet run
```

Swagger UI: [http://localhost:5000/swagger/index.html](http://localhost:5000/swagger/index.html)

## Endpoints

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/weather/temperature` | Average air temperature across all SMHI stations for the latest hour |
| GET | `/api/weather/rainfall/{city}` | Total rainfall in mm for a Swedish city over the latest ~4 months |

**GET /api/weather/temperature**
```json
{ "averageTemperatureInCelsius": 12.3 }
```

**GET /api/weather/rainfall/Lund**
```json
{
  "rainfallInMillimeters": 203.3,
  "startDate": "2026-01-22",
  "endDate": "2026-05-31"
}
```

`startDate`/`endDate` reflect the actual span of data returned by SMHI — no fixed window.

| Condition | Status |
|-----------|--------|
| Unknown city | 404 with RFC 7807 body |
| Blank/whitespace city | 400 |
| SMHI unreachable or no usable data | 502 |

## Testing

```
dotnet test
```

Unit tests cover `WeatherService` and `StationResolver` against stub/fake implementations — no live HTTP calls.

## Configuration

`appsettings.json`:

```json
"WeatherApi": {
  "SmhiBaseUrl": "https://opendata-download-metobs.smhi.se/api/version/latest"
}
```
