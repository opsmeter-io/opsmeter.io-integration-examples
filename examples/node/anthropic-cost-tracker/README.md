# Anthropic Cost Tracker Example

Minimal Node.js example for one narrow promise:

1. Call Anthropic.
2. Read token usage from the response.
3. Send request-level telemetry to Opsmeter with `endpointTag`, `promptVersion`, and optional `tenantId`.

This is a provider-specific launch wedge for the same provider-agnostic telemetry pattern used elsewhere in the repo.

## Why this example exists

- Shows a real Anthropic Messages API call instead of a synthetic payload-only flow
- Makes the Opsmeter value obvious in one script
- Demonstrates the minimum metadata needed for attribution
- Makes it clear that the provider can change while the Opsmeter payload stays stable

## What it demonstrates

- Anthropic Messages API call from Node.js
- Async direct-ingest to `POST /v1/ingest/llm-request`
- Stable tagging with `endpointTag` and `promptVersion`
- Optional `tenantId` and `featureTag`
- Error-path telemetry so failed requests still produce operational context

## Files

- `index.mjs`: runnable example
- `.env.example`: required environment variables
- `package.json`: isolated dependencies for this example

## Quickstart

```bash
cd examples/node/anthropic-cost-tracker
npm install
cp .env.example .env
npm start
```

Required environment variables:

- `ANTHROPIC_API_KEY`
- `OPSMETER_API_KEY`

Optional environment variables:

- `ANTHROPIC_MODEL` default: `claude-3-5-sonnet-20241022`
- `OPSMETER_API_BASE_URL` default: `https://api.opsmeter.io`
- `OPSMETER_ENDPOINT_TAG` default: `support.reply`
- `OPSMETER_PROMPT_VERSION` default: `support_reply_v2`
- `OPSMETER_TENANT_ID`
- `OPSMETER_FEATURE_TAG`
- `OPSMETER_ENVIRONMENT` default: `prod`
- `OPSMETER_DATA_MODE` default: `real`
- `EXAMPLE_INPUT`

## Expected output

After one run you should see:

- Model output in the terminal
- Token usage summary in the terminal
- One telemetry row in Opsmeter
- Attribution dimensions populated for endpoint and prompt version

## When to use the generic Node example instead

Use [`../index.mjs`](../index.mjs) when you want the smallest possible direct-ingest example with no provider SDK dependency and deterministic retry demonstration.
