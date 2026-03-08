using Opsmeter.IntegrationExamples;

var options = CliOptions.Parse(args);
var apiKey = Environment.GetEnvironmentVariable("OPSMETER_API_KEY") ?? string.Empty;
var apiBaseUrl = Environment.GetEnvironmentVariable("OPSMETER_API_BASE_URL") ?? "https://api.opsmeter.io";
var dryRun = string.Equals(Environment.GetEnvironmentVariable("OPSMETER_DRY_RUN"), "1", StringComparison.Ordinal);

if (string.IsNullOrWhiteSpace(apiKey) && !dryRun)
{
    Console.Error.WriteLine("Missing OPSMETER_API_KEY");
    return 1;
}

var payload = PayloadFactory.Build(
    operationKey: options.OperationKey,
    provider: options.Provider,
    model: options.Model,
    dataMode: options.DataMode,
    environment: options.Environment,
    userId: null);

Console.WriteLine("Business call completed.");

using var httpClient = new HttpClient();
var telemetryClient = new TelemetryClient(httpClient);

var firstSend = telemetryClient.SendSafeAsync(
    payload,
    apiBaseUrl,
    apiKey,
    timeoutMs: 800,
    dryRun,
    CancellationToken.None);

Console.WriteLine("Telemetry dispatched (non-blocking).");
var firstResult = await firstSend;
PrintResult("Attempt 1", firstResult);

if (options.Retry)
{
    var retry = await telemetryClient.SendSafeAsync(
        payload,
        apiBaseUrl,
        apiKey,
        timeoutMs: 800,
        dryRun,
        CancellationToken.None);

    PrintResult("Retry with same externalRequestId", retry);
}

Console.WriteLine($"externalRequestId={payload.ExternalRequestId}");
return 0;

static void PrintResult(string label, TelemetrySendResult result)
{
    if (!result.Ok)
    {
        Console.WriteLine($"{label}: telemetry failed but business flow continues -> {result.Error}");
        return;
    }

    var warningCount = result.Body?.Warnings?.Count ?? 0;
    Console.WriteLine($"{label}: ingest response {result.StatusCode} ok={result.Body?.Ok ?? false} planTier={result.Body?.PlanTier ?? "n/a"} warnings={warningCount}");
}

internal sealed record CliOptions(
    string Provider,
    string Model,
    string OperationKey,
    string DataMode,
    string Environment,
    bool Retry)
{
    public static CliOptions Parse(string[] args)
    {
        // Provider/model names are in catalog: https://opsmeter.io/docs/catalog
        var provider = "openai";
        var model = "gpt-4o-mini";
        var operationKey = "order:1001";
        var dataMode = "real";
        var environment = "prod";
        var retry = false;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--provider" when i + 1 < args.Length:
                    provider = args[++i];
                    break;
                case "--model" when i + 1 < args.Length:
                    model = args[++i];
                    break;
                case "--operation-key" when i + 1 < args.Length:
                    operationKey = args[++i];
                    break;
                case "--data-mode" when i + 1 < args.Length:
                    dataMode = args[++i];
                    break;
                case "--environment" when i + 1 < args.Length:
                    environment = args[++i];
                    break;
                case "--retry":
                    retry = true;
                    break;
            }
        }

        provider = provider.ToLowerInvariant();
        if (provider is not ("openai" or "anthropic"))
        {
            throw new ArgumentException("Supported providers: openai, anthropic");
        }

        return new CliOptions(provider, model, operationKey, dataMode, environment, retry);
    }
}
