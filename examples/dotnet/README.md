# Dotnet examples

This folder includes both integration modes:

- **Without SDK (direct ingest):** existing runnable .NET sample in this folder.
- **With SDK (preview):** usage sample under `with-sdk/`.

## 1) Without SDK (direct ingest)

Use provider/model names from: [https://opsmeter.io/docs/catalog](https://opsmeter.io/docs/catalog)
Supported providers in this example: **OpenAI** and **Anthropic** only.

Run:

```bash
export OPSMETER_API_KEY="<YOUR_KEY>"
export OPSMETER_API_BASE_URL="https://api.opsmeter.io"
# Provider/model names: https://opsmeter.io/docs/catalog
dotnet run --project examples/dotnet/src/Opsmeter.IntegrationExamples -- --provider openai --model gpt-4o-mini --retry
```

Anthropic sample:

```bash
# Provider/model names: https://opsmeter.io/docs/catalog
dotnet run --project examples/dotnet/src/Opsmeter.IntegrationExamples -- --provider anthropic --model claude-3-5-sonnet-20241022 --data-mode test --environment staging
```

Dry-run:

```bash
OPSMETER_DRY_RUN=1 OPSMETER_API_KEY=dummy dotnet run --project examples/dotnet/src/Opsmeter.IntegrationExamples -- --retry
```

More: [`without-sdk/README.md`](./without-sdk/README.md)

## 2) With SDK (preview)

See usage sample:

- [`with-sdk/Program.cs`](./with-sdk/Program.cs)
- [`with-sdk/README.md`](./with-sdk/README.md)
- Provider/model names should match catalog entries: [https://opsmeter.io/docs/catalog](https://opsmeter.io/docs/catalog)
- OpenAI send path is included in `with-sdk/Program.cs` (`--provider openai` branch)
- Anthropic send path is included in `with-sdk/Program.cs` (`--provider anthropic` branch)

SDK package/repo:

- package: coming soon
- repo: coming soon
