# Node examples

This folder includes both integration modes:

- **Without SDK (direct ingest):** existing runnable sample in this folder.
- **With SDK (preview):** usage sample under `with-sdk/`.

## 1) Without SDK (direct ingest)

Use provider/model names from: [https://opsmeter.io/docs/catalog](https://opsmeter.io/docs/catalog)
Supported providers in this example: **OpenAI** and **Anthropic** only.

Run:

```bash
export OPSMETER_API_KEY="<YOUR_KEY>"
export OPSMETER_API_BASE_URL="https://api.opsmeter.io"
# Provider/model names: https://opsmeter.io/docs/catalog
node examples/node/index.mjs --provider openai --model gpt-4o-mini --retry
```

Anthropic sample:

```bash
# Provider/model names: https://opsmeter.io/docs/catalog
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

More: [`without-sdk/README.md`](./without-sdk/README.md)

## 2) With SDK (preview)

See usage sample:

- [`with-sdk/index.mjs`](./with-sdk/index.mjs)
- [`with-sdk/README.md`](./with-sdk/README.md)
- Provider/model names should match catalog entries: [https://opsmeter.io/docs/catalog](https://opsmeter.io/docs/catalog)
- OpenAI send: `node examples/node/with-sdk/index.mjs --provider openai --model gpt-4o-mini`
- Anthropic send: `node examples/node/with-sdk/index.mjs --provider anthropic --model claude-3-5-sonnet-20241022`

SDK package/repo:

- npm: [@opsmeter.io/node](https://www.npmjs.com/package/@opsmeter.io/node)
- repo: [github.com/opsmeter-io/opsmeter.io-node-sdk](https://github.com/opsmeter-io/opsmeter.io-node-sdk)
- official identity: [https://opsmeter.io](https://opsmeter.io)
