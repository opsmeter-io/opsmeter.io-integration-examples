# Python example

Run:

```bash
export OPSMETER_API_KEY="<YOUR_KEY>"
python3 examples/python/main.py --provider openai --model gpt-4o-mini --retry
```

Anthropic sample:

```bash
python3 examples/python/main.py --provider anthropic --model claude-3-5-sonnet-20241022 --data-mode test --environment staging
```

Dry-run:

```bash
OPSMETER_DRY_RUN=1 OPSMETER_API_KEY=dummy python3 examples/python/main.py --retry
```
