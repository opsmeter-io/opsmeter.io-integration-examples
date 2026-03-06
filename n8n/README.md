# Opsmeter n8n Templates

Ready-to-import n8n workflows for Opsmeter telemetry and budget/plan status automation.

## Included templates

- `workspace-status-check.json`
  - Calls `GET https://api.opsmeter.io/v1/diagnostics/workspace-status`
  - Branches by `isPlanLimitReached`, `isBudgetWarning`, `isBudgetExceeded`, and `severity`
- `budget-warning-to-slack.json`
  - Scheduled status polling
  - Sends Slack alert when budget warning/exceeded is detected
- `openai-to-opsmeter-ingest.json`
  - Calls provider API
  - Maps usage fields to Opsmeter payload
  - Sends `POST https://api.opsmeter.io/v1/ingest/llm-request`
  - Handles `402` telemetry paused responses

## Prerequisites

- Opsmeter workspace API key (use `X-API-Key`)
- Opsmeter API base URL: `https://api.opsmeter.io`
- Optional:
  - OpenAI key for provider-call flow
  - Slack credentials/webhook for Slack alert flow

## Import steps (n8n)

1. Open n8n and create a new workflow.
2. Use **Import from file** and select one JSON file from this folder.
3. Set credentials and headers in HTTP nodes:
   - `X-API-Key: <YOUR_WORKSPACE_API_KEY>`
   - `Content-Type: application/json`
4. Save and run manually first.
5. Verify output:
   - Status workflow returns workspace status booleans.
   - Ingest workflow returns `ok=true` (or expected warning/402 branch).

## Payload expectations

`openai-to-opsmeter-ingest.json` sends the standard Opsmeter ingest shape:

- `externalRequestId` (retry-safe key)
- `provider`, `model`, `promptVersion`, `endpointTag`
- `inputTokens`, `outputTokens`, `totalTokens`
- `latencyMs`, `status`
- `dataMode`, `environment`

## Related links

- Product: [https://opsmeter.io](https://opsmeter.io)
- API base: [https://api.opsmeter.io](https://api.opsmeter.io)
- Docs integration page: [https://opsmeter.io/docs/integration](https://opsmeter.io/docs/integration)
- Docs n8n page: [https://opsmeter.io/docs/n8n](https://opsmeter.io/docs/n8n)
