# Dotnet example

Run:

```bash
export OPSMETER_API_KEY="<YOUR_KEY>"
export OPSMETER_API_BASE_URL="https://api.opsmeter.io"
dotnet run --project examples/dotnet/src/Opsmeter.IntegrationExamples -- --provider openai --model gpt-4o-mini --retry
```

Anthropic sample:

```bash
dotnet run --project examples/dotnet/src/Opsmeter.IntegrationExamples -- --provider anthropic --model claude-3-5-sonnet-20241022 --data-mode test --environment staging
```

Dry-run:

```bash
OPSMETER_DRY_RUN=1 OPSMETER_API_KEY=dummy dotnet run --project examples/dotnet/src/Opsmeter.IntegrationExamples -- --retry
```
