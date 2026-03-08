using Opsmeter.Sdk;

// Provider/model names are in catalog: https://opsmeter.io/docs/catalog
var provider = "openai";
var model = "gpt-4o-mini";

for (var i = 0; i < args.Length; i++)
{
    if (args[i] == "--provider" && i + 1 < args.Length)
    {
        provider = args[++i].ToLowerInvariant();
    }
    else if (args[i] == "--model" && i + 1 < args.Length)
    {
        model = args[++i];
    }
}

provider = provider.ToLowerInvariant();
if (provider is not ("openai" or "anthropic"))
{
    throw new ArgumentException("Supported providers: openai, anthropic");
}

if (provider == "anthropic" && model == "gpt-4o-mini")
{
    model = "claude-3-5-sonnet-20241022";
}

OpsmeterSdk.Init(new OpsmeterOptions
{
    ApiKey = Environment.GetEnvironmentVariable("OPSMETER_API_KEY") ?? "",
    WorkspaceId = "ws_123",
    Environment = "prod"
});

if (provider == "anthropic")
{
    var captured = await OpsmeterSdk.WithContext(
        new TelemetryContext
        {
            UserId = "u_1",
            TenantId = "tenant_a",
            Endpoint = "/api/support",
            Feature = "support",
            PromptVersion = "v8"
        },
        async () => await OpsmeterSdk.CaptureAnthropicMessageWithResultAsync(
            providerCall: () => Task.FromResult<IDictionary<string, object?>>(new Dictionary<string, object?>
            {
                ["id"] = "msg_1",
                ["model"] = model,
                ["usage"] = new Dictionary<string, object?>
                {
                    ["input_tokens"] = 120,
                    ["output_tokens"] = 45
                }
            }),
            options: new OpenAiCaptureOptions
            {
                Model = model,
                AwaitTelemetryResponse = true
            })
    );

    Console.WriteLine($"Provider: {captured.Payload.Provider}");
    Console.WriteLine($"Provider model: {captured.ProviderResponse["model"]}");
    Console.WriteLine($"Telemetry status: {captured.Telemetry.Status}");
}
else
{
    var captured = await OpsmeterSdk.WithContext(
        new TelemetryContext
        {
            UserId = "u_1",
            TenantId = "tenant_a",
            Endpoint = "/api/chat",
            Feature = "assistant",
            PromptVersion = "v12"
        },
        async () => await OpsmeterSdk.CaptureOpenAiChatCompletionWithResultAsync(
            providerCall: () => Task.FromResult<IDictionary<string, object?>>(new Dictionary<string, object?>
            {
                ["id"] = "req_1",
                ["model"] = model,
                ["usage"] = new Dictionary<string, object?>
                {
                    ["prompt_tokens"] = 120,
                    ["completion_tokens"] = 45,
                    ["total_tokens"] = 165
                }
            }),
            options: new OpenAiCaptureOptions
            {
                Model = model,
                AwaitTelemetryResponse = true
            })
    );

    Console.WriteLine($"Provider: {captured.Payload.Provider}");
    Console.WriteLine($"Provider model: {captured.ProviderResponse["model"]}");
    Console.WriteLine($"Telemetry status: {captured.Telemetry.Status}");
}
