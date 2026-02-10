using Opsmeter.IntegrationExamples;

namespace Opsmeter.IntegrationExamples.Tests;

public class PayloadFactoryTests
{
    [Fact]
    public void BuildExternalRequestId_IsDeterministic()
    {
        var first = PayloadFactory.BuildExternalRequestId("order:1001");
        var second = PayloadFactory.BuildExternalRequestId("order:1001");

        Assert.Equal(first, second);
        Assert.StartsWith("ext_", first);
    }

    [Fact]
    public void Build_RetryUsesSameExternalRequestId_WhenOperationKeyMatches()
    {
        var first = PayloadFactory.Build(
            operationKey: "invoice:42",
            provider: "openai",
            model: "gpt-4o-mini",
            dataMode: "real",
            environment: "prod");

        var retry = PayloadFactory.Build(
            operationKey: "invoice:42",
            provider: "openai",
            model: "gpt-4o-mini",
            dataMode: "real",
            environment: "prod");

        Assert.Equal(first.ExternalRequestId, retry.ExternalRequestId);
    }

    [Fact]
    public void Build_SupportsAnthropicTestPayload()
    {
        var payload = PayloadFactory.Build(
            operationKey: "support:101",
            provider: "anthropic",
            model: "claude-3-5-sonnet-20241022",
            dataMode: "test",
            environment: "staging");

        Assert.Equal("anthropic", payload.Provider);
        Assert.Equal("support_reply_v2", payload.PromptVersion);
        Assert.Equal("support.reply", payload.EndpointTag);
        Assert.Equal("test", payload.DataMode);
        Assert.Equal("staging", payload.Environment);
    }
}
