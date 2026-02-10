using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Opsmeter.IntegrationExamples;

public sealed class TelemetryClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public TelemetryClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TelemetrySendResult> SendSafeAsync(
        IngestPayload payload,
        string apiBaseUrl,
        string apiKey,
        int timeoutMs,
        bool dryRun,
        CancellationToken cancellationToken)
    {
        if (dryRun)
        {
            return new TelemetrySendResult(
                Ok: true,
                StatusCode: 200,
                Body: new IngestResult
                {
                    Ok = true,
                    PlanTier = "Free",
                    Warnings = new List<string> { "dry-run mode: request not sent" }
                },
                Error: null);
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return new TelemetrySendResult(false, 0, null, "Missing OPSMETER_API_KEY");
        }

        var endpoint = $"{apiBaseUrl.TrimEnd('/')}/v1/ingest/llm-request";
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromMilliseconds(timeoutMs));

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(JsonSerializer.Serialize(payload, _jsonOptions), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("X-API-Key", apiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await _httpClient.SendAsync(request, timeoutCts.Token);
            var bodyText = await response.Content.ReadAsStringAsync(timeoutCts.Token);
            IngestResult? body = null;
            if (!string.IsNullOrWhiteSpace(bodyText))
            {
                body = JsonSerializer.Deserialize<IngestResult>(bodyText, _jsonOptions);
            }

            return new TelemetrySendResult(
                Ok: response.IsSuccessStatusCode,
                StatusCode: (int)response.StatusCode,
                Body: body,
                Error: response.IsSuccessStatusCode ? null : bodyText);
        }
        catch (Exception ex)
        {
            // Swallow telemetry failures so business flow can continue.
            return new TelemetrySendResult(false, 0, null, ex.Message);
        }
    }
}
