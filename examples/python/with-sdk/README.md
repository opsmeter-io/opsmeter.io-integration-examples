# Python with SDK (preview)

This sample shows the `init + context` model using `opsmeter-io-sdk`.
Provider/model names are from the catalog: [https://opsmeter.io/docs/catalog](https://opsmeter.io/docs/catalog)
Supported providers in this sample: **OpenAI** and **Anthropic** only.
Official package/source of truth: [https://opsmeter.io](https://opsmeter.io)
Model catalog (required for provider/model names): [https://opsmeter.io/docs/catalog](https://opsmeter.io/docs/catalog)

## Install

```bash
pip install opsmeter-io-sdk openai anthropic
```

## Run pattern

```bash
export OPSMETER_API_KEY="<YOUR_OPSMETER_KEY>"
export OPENAI_API_KEY="<YOUR_OPENAI_KEY>"
# Provider/model names: https://opsmeter.io/docs/catalog
python3 examples/python/with-sdk/main.py --provider openai --model gpt-4o-mini
```

```bash
export OPSMETER_API_KEY="<YOUR_OPSMETER_KEY>"
export ANTHROPIC_API_KEY="<YOUR_ANTHROPIC_KEY>"
# Provider/model names: https://opsmeter.io/docs/catalog
python3 examples/python/with-sdk/main.py --provider anthropic --model claude-3-5-sonnet-20241022
```

Provider calls remain direct; SDK captures telemetry and emits asynchronously.
