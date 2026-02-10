using System.Text.Json.Serialization;

namespace Opsmeter.IntegrationExamples;

public sealed class IngestPayload
{
    [JsonPropertyName("externalRequestId")]
    public string ExternalRequestId { get; set; } = string.Empty;

    [JsonPropertyName("provider")]
    public string Provider { get; set; } = string.Empty;

    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("promptVersion")]
    public string PromptVersion { get; set; } = string.Empty;

    [JsonPropertyName("endpointTag")]
    public string EndpointTag { get; set; } = string.Empty;

    [JsonPropertyName("inputTokens")]
    public int InputTokens { get; set; }

    [JsonPropertyName("outputTokens")]
    public int OutputTokens { get; set; }

    [JsonPropertyName("totalTokens")]
    public int TotalTokens { get; set; }

    [JsonPropertyName("latencyMs")]
    public int LatencyMs { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = "success";

    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    [JsonPropertyName("dataMode")]
    public string DataMode { get; set; } = "real";

    [JsonPropertyName("environment")]
    public string Environment { get; set; } = "prod";
}

public sealed class IngestResult
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("planTier")]
    public string? PlanTier { get; set; }

    [JsonPropertyName("warnings")]
    public List<string> Warnings { get; set; } = new();
}

public sealed record TelemetrySendResult(bool Ok, int StatusCode, IngestResult? Body, string? Error);
