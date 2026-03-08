# Node with SDK (preview)

This sample shows the `init + withContext` model using `@opsmeter/node`.
Provider/model names are from the catalog: [https://opsmeter.io/docs/catalog](https://opsmeter.io/docs/catalog)
Supported providers in this sample: **OpenAI** and **Anthropic** only.
Official package/source of truth: [https://opsmeter.io](https://opsmeter.io)
Model catalog (required for provider/model names): [https://opsmeter.io/docs/catalog](https://opsmeter.io/docs/catalog)

## Install

```bash
npm install @opsmeter/node openai @anthropic-ai/sdk
```

## Run pattern

```bash
export OPSMETER_API_KEY="<YOUR_OPSMETER_KEY>"
export OPENAI_API_KEY="<YOUR_OPENAI_KEY>"
# Provider/model names: https://opsmeter.io/docs/catalog
node examples/node/with-sdk/index.mjs --provider openai --model gpt-4o-mini
```

```bash
export OPSMETER_API_KEY="<YOUR_OPSMETER_KEY>"
export ANTHROPIC_API_KEY="<YOUR_ANTHROPIC_KEY>"
# Provider/model names: https://opsmeter.io/docs/catalog
node examples/node/with-sdk/index.mjs --provider anthropic --model claude-3-5-sonnet-20241022
```

The SDK still keeps provider calls direct. It observes supported calls and sends Opsmeter telemetry asynchronously.
