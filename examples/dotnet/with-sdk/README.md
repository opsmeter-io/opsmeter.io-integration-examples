# Dotnet with SDK (preview)

This sample shows the `Init + WithContext` model using `Opsmeter.Sdk`.
Provider/model names are from the catalog: [https://opsmeter.io/docs/catalog](https://opsmeter.io/docs/catalog)
Supported providers in this sample: **OpenAI** and **Anthropic** only.

## Install

```bash
dotnet add package Opsmeter.Sdk --prerelease
```

## Run pattern

Use the code in `Program.cs` as a drop-in pattern for your ASP.NET/worker app flow.
It includes both variants:

- OpenAI send path (`--provider openai`)
- Anthropic send path (`--provider anthropic`)

Provider calls remain direct; SDK captures telemetry and emits asynchronously.
