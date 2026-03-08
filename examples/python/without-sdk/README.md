# Python without SDK (direct ingest)

This mode sends telemetry directly to:

- `POST /v1/ingest/llm-request`

Entry points:

- `../main.py`
- `../telemetry.py`

Use this mode when you want explicit payload mapping and no SDK dependency.
