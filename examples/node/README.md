# Node example

Run:

```bash
export OPSMETER_API_KEY="<YOUR_KEY>"
node examples/node/index.mjs --provider openai --model gpt-4o-mini --retry
```

Anthropic sample:

```bash
node examples/node/index.mjs --provider anthropic --model claude-3-5-sonnet-20241022 --data-mode test --environment staging
```

Flags:
- `--operation-key` deterministic source for retry-safe `externalRequestId`
- `--retry` sends same payload twice to demonstrate idempotent retry behavior
- `--data-mode real|test|demo`
- `--environment prod|staging|dev`

Dry-run (CI smoke):

```bash
OPSMETER_DRY_RUN=1 OPSMETER_API_KEY=dummy node examples/node/index.mjs --retry
```
