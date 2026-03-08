# Node without SDK (direct ingest)

This mode sends telemetry directly to:

- `POST /v1/ingest/llm-request`

Entry points:

- `../index.mjs`
- `../telemetry.mjs`

Use this mode when you want explicit payload control and no SDK dependency.
