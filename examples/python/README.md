# Python examples

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
python3 examples/python/main.py --provider openai --model gpt-4o-mini --retry
```

Anthropic sample:

```bash
# Provider/model names: https://opsmeter.io/docs/catalog
python3 examples/python/main.py --provider anthropic --model claude-3-5-sonnet-20241022 --data-mode test --environment staging
```

Dry-run:

```bash
OPSMETER_DRY_RUN=1 OPSMETER_API_KEY=dummy python3 examples/python/main.py --retry
```

More: [`without-sdk/README.md`](./without-sdk/README.md)

## 2) With SDK (preview)

See usage sample:

- [`with-sdk/main.py`](./with-sdk/main.py)
- [`with-sdk/README.md`](./with-sdk/README.md)
- Provider/model names should match catalog entries: [https://opsmeter.io/docs/catalog](https://opsmeter.io/docs/catalog)
- OpenAI send: `python3 examples/python/with-sdk/main.py --provider openai --model gpt-4o-mini`
- Anthropic send: `python3 examples/python/with-sdk/main.py --provider anthropic --model claude-3-5-sonnet-20241022`

SDK package/repo:

- package: [opsmeter-io-sdk](https://pypi.org/project/opsmeter-io-sdk/)
- repo: [github.com/opsmeter-io/opsmeter.io-python-sdk](https://github.com/opsmeter-io/opsmeter.io-python-sdk)
- model catalog: [https://opsmeter.io/docs/catalog](https://opsmeter.io/docs/catalog)
- official identity: [https://opsmeter.io](https://opsmeter.io)
