import crypto from 'node:crypto';

const VALID_DATA_MODES = new Set(['real', 'test', 'demo']);
const VALID_ENVIRONMENTS = new Set(['prod', 'staging', 'dev']);

export function buildExternalRequestId(operationKey) {
  const normalized = (operationKey || '').trim().toLowerCase();
  if (!normalized) {
    throw new Error('operationKey is required for externalRequestId generation.');
  }

  const hash = crypto.createHash('sha256').update(normalized).digest('hex').slice(0, 24);
  return `ext_${hash}`;
}

function normalizeDataMode(dataMode) {
  const normalized = (dataMode || 'real').trim().toLowerCase();
  if (!VALID_DATA_MODES.has(normalized)) {
    throw new Error(`Invalid dataMode: ${dataMode}. Allowed: real|test|demo`);
  }

  return normalized;
}

function normalizeEnvironment(environment) {
  const normalized = (environment || 'prod').trim().toLowerCase();
  if (!VALID_ENVIRONMENTS.has(normalized)) {
    throw new Error(`Invalid environment: ${environment}. Allowed: prod|staging|dev`);
  }

  return normalized;
}

export function buildPayload({
  operationKey,
  provider,
  model,
  userId = null,
  status = 'success',
  dataMode = 'real',
  environment = 'prod'
}) {
  const safeProvider = (provider || '').trim();
  const safeModel = (model || '').trim();
  if (!safeProvider || !safeModel) {
    throw new Error('provider and model are required.');
  }

  const inputTokens = 120;
  const outputTokens = 45;
  const latencyMs = 820;

  return {
    externalRequestId: buildExternalRequestId(operationKey),
    provider: safeProvider,
    model: safeModel,
    promptVersion: safeProvider === 'anthropic' ? 'support_reply_v2' : 'summary_v3',
    endpointTag: safeProvider === 'anthropic' ? 'support.reply' : 'checkout.ai_summary',
    inputTokens,
    outputTokens,
    totalTokens: inputTokens + outputTokens,
    latencyMs,
    status,
    errorCode: status === 'error' ? 'provider_timeout' : null,
    userId,
    dataMode: normalizeDataMode(dataMode),
    environment: normalizeEnvironment(environment)
  };
}

export async function sendTelemetry(payload, {
  apiBaseUrl,
  apiKey,
  timeoutMs = 800,
  dryRun = false
}) {
  if (dryRun) {
    return {
      ok: true,
      status: 200,
      body: {
        ok: true,
        planTier: 'Free',
        warnings: ['dry-run mode: request was not sent']
      }
    };
  }

  const baseUrl = (apiBaseUrl || '').trim() || 'https://api.opsmeter.io';
  if (!apiKey) {
    throw new Error('OPSMETER_API_KEY is required.');
  }

  const endpoint = `${baseUrl.replace(/\/$/, '')}/v1/ingest/llm-request`;
  const controller = new AbortController();
  const timeout = setTimeout(() => controller.abort(), timeoutMs);

  try {
    const response = await fetch(endpoint, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'X-API-Key': apiKey
      },
      body: JSON.stringify(payload),
      signal: controller.signal
    });

    const bodyText = await response.text();
    let body;
    try {
      body = JSON.parse(bodyText);
    } catch {
      body = { raw: bodyText };
    }

    return {
      ok: response.ok,
      status: response.status,
      body
    };
  } catch (error) {
    return {
      ok: false,
      status: 0,
      error: error instanceof Error ? error.message : String(error)
    };
  } finally {
    clearTimeout(timeout);
  }
}
