import "dotenv/config";
import OpenAI from "openai";
import { randomUUID } from "node:crypto";
import { sendTelemetry } from "../telemetry.mjs";

const requiredEnvVars = ["OPENAI_API_KEY", "OPSMETER_API_KEY"];

for (const key of requiredEnvVars) {
  if (!process.env[key]) {
    throw new Error(`Missing required environment variable: ${key}`);
  }
}

const config = {
  model: process.env.OPENAI_MODEL || "gpt-4o-mini",
  opsmeterBaseUrl: (process.env.OPSMETER_API_BASE_URL || process.env.OPSMETER_BASE_URL || "https://api.opsmeter.io").replace(/\/$/, ""),
  endpointTag: process.env.OPSMETER_ENDPOINT_TAG || "support.reply",
  promptVersion: process.env.OPSMETER_PROMPT_VERSION || "support_v1",
  tenantId: process.env.OPSMETER_TENANT_ID || "",
  featureTag: process.env.OPSMETER_FEATURE_TAG || "",
  environment: process.env.OPSMETER_ENVIRONMENT || "prod",
  dataMode: process.env.OPSMETER_DATA_MODE || "real",
  exampleInput:
    process.env.EXAMPLE_INPUT ||
    "Summarize why endpointTag and promptVersion matter for AI cost control in three bullets."
};

const client = new OpenAI({ apiKey: process.env.OPENAI_API_KEY });

function buildBasePayload(externalRequestId, latencyMs) {
  return {
    externalRequestId,
    provider: "openai",
    model: config.model,
    promptVersion: config.promptVersion,
    endpointTag: config.endpointTag,
    latencyMs,
    dataMode: config.dataMode,
    environment: config.environment,
    createdAt: new Date().toISOString(),
    ...(config.tenantId ? { tenantId: config.tenantId } : {}),
    ...(config.featureTag ? { featureTag: config.featureTag } : {})
  };
}

function usageToTelemetry(responseUsage) {
  const inputTokens = responseUsage?.input_tokens ?? 0;
  const outputTokens = responseUsage?.output_tokens ?? 0;
  const totalTokens = responseUsage?.total_tokens ?? inputTokens + outputTokens;

  return {
    inputTokens,
    outputTokens,
    totalTokens
  };
}

async function ingestPayload(payload) {
  return sendTelemetry(payload, {
    apiBaseUrl: config.opsmeterBaseUrl,
    apiKey: process.env.OPSMETER_API_KEY,
    timeoutMs: 1500,
    dryRun: false
  });
}

async function main() {
  const externalRequestId = randomUUID();
  const startedAt = Date.now();

  try {
    const response = await client.responses.create({
      model: config.model,
      input: config.exampleInput
    });

    const latencyMs = Date.now() - startedAt;
    const usage = usageToTelemetry(response.usage);
    const payload = {
      ...buildBasePayload(externalRequestId, latencyMs),
      model: response.model || config.model,
      ...usage,
      status: "success",
      errorCode: null
    };

    const ingestResponse = await ingestPayload(payload);

    console.log("\nModel output:\n");
    console.log(response.output_text || "(no output_text returned)");
    console.log("\nUsage:");
    console.table(usage);
    console.log("\nOpsmeter payload:");
    console.log(payload);
    console.log("\nIngest response:");
    console.log(ingestResponse);
  } catch (error) {
    const latencyMs = Date.now() - startedAt;

    await ingestPayload({
      ...buildBasePayload(externalRequestId, latencyMs),
      inputTokens: 0,
      outputTokens: 0,
      totalTokens: 0,
      status: "error",
      errorCode: error instanceof Error ? error.name : "openai_request_failed"
    });

    throw error;
  }
}

main().catch((error) => {
  console.error("\nExample failed:");
  console.error(error instanceof Error ? error.message : String(error));
  process.exitCode = 1;
});
