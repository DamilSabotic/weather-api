# Design decisions

Short rationale for the non-obvious trade-offs. Facts below were checked against the live
SMHI MetObs API.

## City to station matching (active-first)
We resolve `{city}` against SMHI's own precipitation station catalogue (`parameter/5.json`) by
name, not via a geocoder. Names are compared lowercased and diacritic-folded (`å/ä->a, ö->o, é->e`)
so `Goteborg` resolves to `Göteborg`. Candidates are exact folded matches or prefix matches up to a
word boundary (avoids `Boda` matching `Långsboda`).

Tie-break is `(active, exact, lowest id)` - **active first**. For major cities SMHI keeps the modern
automatic station under a `" A"` suffix (e.g. `Göteborg A`, `Malmö A`) and marks the bare-name manual
predecessor inactive with no recent data. Preferring exact-name-first would resolve `Göteborg`/`Malmö`
(the brief's own examples) to inactive stations that 404, so active-first is the correct rule. `Lund`
is active and exact, so it still wins.

Limitation (accepted): a real city without a same-named station yields 404; the resolved station name
and id are logged so unexpected matches are diagnosable.

## "Total rainfall for the last months"
There is no precise window, and the brief states "your result may differ", so we do not invent one.
We sum the `latest-months` payload (SMHI's last ~4 months of daily sums) and report `startDate`/
`endDate` as the actual span of the data summed - no `UtcNow` arithmetic. The sample's 2024 figures
are not reproduced because `latest-months` only serves recent data; the sample is treated as
shape-only.

## Value parsing / no quality filtering
SMHI returns all observation values as strings, parsed with `InvariantCulture`. Null/empty/
non-numeric values are excluded. We deliberately do **not** filter on the `quality` code: real-time
data is effectively always `G`/`Y`, and an undefined "drop invalid quality" rule risks dropping valid
data.

## Error model
- Unknown city -> `CityNotFoundException` -> 404 (RFC 7807).
- Blank/whitespace city -> `ArgumentException` -> 400.
- Upstream success but no usable data (empty station set, no usable observations) ->
  `SmhiUnavailableException` -> 502. Mapping a 200-but-empty payload to 502 is a conscious choice:
  it is an unusable upstream response.
- Transport faults / non-success / timeouts propagate and map centrally to 502 in
  `GlobalExceptionHandler`; a client disconnect writes no response.

## Authentication removed
The template wired Windows Negotiate with a fallback policy that 401s anonymous GETs. This API serves
public open data with no per-user identity and must run for any reviewer (Linux/CI/macOS), so Negotiate
was removed in favour of anonymous access. An API key/JWT would be the right follow-up if auth were
ever needed.
