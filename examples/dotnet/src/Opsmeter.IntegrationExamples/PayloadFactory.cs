using System.Security.Cryptography;
using System.Text;

namespace Opsmeter.IntegrationExamples;

public static class PayloadFactory
{
    private static readonly HashSet<string> ValidDataModes = new(StringComparer.OrdinalIgnoreCase)
    {
        "real",
        "test",
        "demo"
    };

    private static readonly HashSet<string> ValidEnvironments = new(StringComparer.OrdinalIgnoreCase)
    {
        "prod",
        "staging",
        "dev"
    };

    public static string BuildExternalRequestId(string operationKey)
    {
        var normalized = (operationKey ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("operationKey is required", nameof(operationKey));
        }

        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(normalized));
        var hash = Convert.ToHexString(hashBytes).ToLowerInvariant()[..24];
        return $"ext_{hash}";
    }

    public static IngestPayload Build(
        string operationKey,
        string provider,
        string model,
        string dataMode = "real",
        string environment = "prod",
        string? userId = null)
    {
        if (string.IsNullOrWhiteSpace(provider))
        {
            throw new ArgumentException("provider is required", nameof(provider));
        }

        if (string.IsNullOrWhiteSpace(model))
        {
            throw new ArgumentException("model is required", nameof(model));
        }

        var normalizedDataMode = NormalizeDataMode(dataMode);
        var normalizedEnvironment = NormalizeEnvironment(environment);
        var providerNormalized = provider.Trim().ToLowerInvariant();

        const int inputTokens = 120;
        const int outputTokens = 45;

        return new IngestPayload
        {
            ExternalRequestId = BuildExternalRequestId(operationKey),
            Provider = provider.Trim(),
            Model = model.Trim(),
            PromptVersion = providerNormalized == "anthropic" ? "support_reply_v2" : "summary_v3",
            EndpointTag = providerNormalized == "anthropic" ? "support.reply" : "checkout.ai_summary",
            InputTokens = inputTokens,
            OutputTokens = outputTokens,
            TotalTokens = inputTokens + outputTokens,
            LatencyMs = 820,
            Status = "success",
            ErrorCode = null,
            UserId = userId,
            DataMode = normalizedDataMode,
            Environment = normalizedEnvironment
        };
    }

    private static string NormalizeDataMode(string dataMode)
    {
        var normalized = (dataMode ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return "real";
        }

        if (!ValidDataModes.Contains(normalized))
        {
            throw new ArgumentException("dataMode must be real|test|demo", nameof(dataMode));
        }

        return normalized;
    }

    private static string NormalizeEnvironment(string environment)
    {
        var normalized = (environment ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return "prod";
        }

        if (!ValidEnvironments.Contains(normalized))
        {
            throw new ArgumentException("environment must be prod|staging|dev", nameof(environment));
        }

        return normalized;
    }
}
