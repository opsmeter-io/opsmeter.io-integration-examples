# OpenAI Cost Tracker Example (Python)

Minimal Python example for one narrow promise:

1. Call OpenAI.
2. Read token usage from the response.
3. Send request-level telemetry to Opsmeter with `endpointTag`, `promptVersion`, and optional `tenantId`.

This is the Python version of the launch-ready GitHub wedge. It is useful when the buyer or evaluator is closer to backend services, internal tools, or ML-flavored Python stacks.

## Why this example exists

- Shows a real OpenAI call instead of a synthetic payload-only flow
- Makes the Opsmeter value obvious in one script
- Demonstrates the minimum metadata needed for attribution
- Easy to link from docs, compare pages, founder posts, and comments

## What it demonstrates

- OpenAI Responses API call from Python
- Async-ish fire-and-forget style direct-ingest to `POST /v1/ingest/llm-request`
- Stable tagging with `endpointTag` and `promptVersion`
- Optional `tenantId` and `featureTag`
- Error-path telemetry so failed requests still produce operational context

## Files

- `main.py`: runnable example
- `.env.example`: required environment variables
- `requirements.txt`: isolated dependencies for this example

## Quickstart

```bash
cd examples/python/openai-cost-tracker
python3 -m venv .venv
source .venv/bin/activate
pip install -r requirements.txt
cp .env.example .env
python3 main.py
```

Required environment variables:

- `OPENAI_API_KEY`
- `OPSMETER_API_KEY`

Optional environment variables:

- `OPENAI_MODEL` default: `gpt-4o-mini`
- `OPSMETER_API_BASE_URL` default: `https://api.opsmeter.io`
- `OPSMETER_ENDPOINT_TAG` default: `support.reply`
- `OPSMETER_PROMPT_VERSION` default: `support_v1`
- `OPSMETER_TENANT_ID`
- `OPSMETER_FEATURE_TAG`
- `OPSMETER_ENVIRONMENT` default: `prod`
- `OPSMETER_DATA_MODE` default: `real`
- `OPSMETER_DRY_RUN` default: `0`
- `EXAMPLE_INPUT`

## Expected output

After one run you should see:

- Model output in the terminal
- Token usage summary in the terminal
- One telemetry row in Opsmeter
- Attribution dimensions populated for endpoint and prompt version

## When to use the generic Python example instead

Use [`../main.py`](../main.py) when you want the smallest possible direct-ingest example with no provider SDK dependency and deterministic retry demonstration.
