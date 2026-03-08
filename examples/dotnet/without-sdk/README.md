# Dotnet without SDK (direct ingest)

This mode sends telemetry directly to:

- `POST /v1/ingest/llm-request`

Entry points:

- `../src/Opsmeter.IntegrationExamples/Program.cs`
- `../src/Opsmeter.IntegrationExamples/TelemetryClient.cs`
- `../src/Opsmeter.IntegrationExamples/PayloadFactory.cs`

Use this mode when you want explicit payload control and no SDK dependency.
