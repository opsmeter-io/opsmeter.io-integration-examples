# Opsmeter Integration Examples

[![Build](https://github.com/opsmeter/opsmeter-integration-examples/actions/workflows/ci.yml/badge.svg?branch=master)](https://github.com/opsmeter/opsmeter-integration-examples/actions/workflows/ci.yml)
[![Version](https://img.shields.io/github/v/tag/opsmeter/opsmeter-integration-examples)](https://github.com/opsmeter/opsmeter-integration-examples/tags)
[![License](https://img.shields.io/github/license/opsmeter/opsmeter-integration-examples)](https://github.com/opsmeter/opsmeter-integration-examples/blob/master/LICENSE)

> **Provider changes, Opsmeter payload stays the same.**

Working examples for sending telemetry to `POST /v1/ingest/llm-request` in:
- .NET (`examples/dotnet`)
- Node.js (`examples/node`)
- Python (`examples/python`)

This repo is optimized for teams implementing **LLM cost tracking**, **OpenAI usage monitoring**, **Anthropic usage telemetry**, and **AI inference cost control** with a consistent request schema.

Opsmeter product site: [https://opsmeter.io](https://opsmeter.io)  
Opsmeter API base: [https://api.opsmeter.io](https://api.opsmeter.io)

## What this repo solves

- **No-proxy telemetry:** keep your provider call path untouched, send attribution metadata after each LLM call.
- **Retry-safe ingestion:** reuse `externalRequestId` on retries to prevent duplicate request rows.
- **Cost attribution dimensions:** keep `endpointTag` and `promptVersion` consistent for feature-level and version-level analysis.

## Table of contents

- [Quickstart (60s)](#quickstart-60s)
- [Payload contract (shared)](#payload-contract-shared)
- [Allowed values](#allowed-values)
- [Recommended combinations](#recommended-combinations)
- [Architecture](#architecture)
- [Quick visual](#quick-visual)
- [Examples](#examples)
- [n8n templates](#n8n-templates)
- [Common mistakes](#common-mistakes)
- [CI / Quality gates](#ci--quality-gates)
- [Product linking text (for docs/pricing/landing)](#product-linking-text-for-docspricinglanding)
- [Release](#release)
- [SEO and discoverability notes](#seo-and-discoverability-notes)

## Quickstart (60s)

1) Clone and set your API key.

```bash
git clone https://github.com/opsmeter/opsmeter-integration-examples.git
cd opsmeter-integration-examples
export OPSMETER_API_KEY="<YOUR_WORKSPACE_PRIMARY_API_KEY>"
export OPSMETER_API_BASE_URL="https://api.opsmeter.io"
```

2) Run one stack (Node shown below):

```bash
node examples/node/index.mjs --provider openai --model gpt-4o-mini --retry
```

3) Expected output:

```text
Business call completed.
Telemetry dispatched (non-blocking).
Ingest response: 200 ok=true planTier=Free warnings=0
Retry with same externalRequestId sent.
```

4) Verify in product:
- Dashboard request count increases.
- `endpointTag` and `promptVersion` appear in Top Endpoints / Prompt Versions.

> `--retry` uses the **same** `externalRequestId` to demonstrate retry-safe behavior.

## Payload contract (shared)

Canonical ingest endpoint: `https://api.opsmeter.io/v1/ingest/llm-request`

All examples send this same shape:

```json
{
  "externalRequestId": "ext_123abc",
  "provider": "openai",
  "model": "gpt-4o-mini",
  "promptVersion": "summary_v3",
  "endpointTag": "checkout.ai_summary",
  "inputTokens": 120,
  "outputTokens": 45,
  "totalTokens": 165,
  "latencyMs": 820,
  "status": "success",
  "errorCode": null,
  "userId": null,
  "dataMode": "real",
  "environment": "prod"
}
```

### Allowed values

| Field | Allowed | Notes |
|---|---|---|
| `status` | `success`, `error` | Required by API validation |
| `dataMode` | `real`, `test`, `demo` | Default recommendation: `real` |
| `environment` | `prod`, `staging`, `dev` | Use real deployment environment |

### Recommended combinations

| Use case | `dataMode` | `environment` |
|---|---|---|
| Production traffic | `real` | `prod` |
| QA/Test traffic | `test` | `staging` or `dev` |
| Seed/demo flows | `demo` | `dev` |

If you do not label these fields correctly, dashboard analytics can mix operational and non-production signals.

## Architecture

```mermaid
flowchart LR
  A["LLM call"] --> B["Map usage + latency"]
  B --> C["Build Opsmeter payload"]
  C --> D["POST /v1/ingest/llm-request"]
  D --> E["Dashboard / Budgets / Alerts"]
```

## Quick visual

![Quickstart example](./assets/quickstart.svg)

## Examples

- [Node example](./examples/node/README.md)
- [Python example](./examples/python/README.md)
- [Dotnet example](./examples/dotnet/README.md)

## n8n templates

These templates are for **Opsmeter n8n integration** with workspace status branching, budget warning automation, and telemetry paused handling.

Path: `./n8n`

- `workspace-status-check.json`: polls `GET /v1/diagnostics/workspace-status` and branches by plan/budget booleans.
- `budget-warning-to-slack.json`: scheduled budget status check with Slack notification path.
- `openai-to-opsmeter-ingest.json`: provider call + usage mapping + ingest + 402 plan-limit branch.
- Import and setup guide: [`n8n/README.md`](./n8n/README.md)

## Common mistakes

> **Common mistakes**
> - Provider/model typo (example: wrong provider string), causing unknown model attribution.
> - Generating a new `externalRequestId` for retries (breaks idempotent behavior).
> - Blocking the request path with long telemetry timeouts.
> - Treating telemetry failure as business failure (it should be swallowed/logged).

## CI / Quality gates

- Node lint + tests
- Python lint + tests
- Dotnet build + tests
- Smoke script runs all three examples in dry-run mode

See `.github/workflows/ci.yml` and `scripts/smoke.sh`.

## Product linking text (for docs/pricing/landing)

Use this exact label when linking from the main product:

`Integration examples (60-second quickstart)`

Target URL:

`https://github.com/opsmeter/opsmeter-integration-examples`

## Release

Current bootstrap release target: **v0.1.0** (see [CHANGELOG](./CHANGELOG.md)).

## SEO and discoverability notes

Primary terms covered in this repository:
- Opsmeter integration examples
- LLM cost tracking integration
- OpenAI usage monitoring
- Anthropic telemetry integration
- AI inference cost control
