import { buildPayload, sendTelemetry } from './telemetry.mjs';

function parseArgs(argv) {
  const options = {
    provider: 'openai',
    model: 'gpt-4o-mini',
    operationKey: 'order:1001',
    dataMode: 'real',
    environment: 'prod',
    retry: false
  };

  for (let i = 0; i < argv.length; i += 1) {
    const arg = argv[i];
    if (arg === '--provider') {
      options.provider = argv[i + 1] || options.provider;
      i += 1;
    } else if (arg === '--model') {
      options.model = argv[i + 1] || options.model;
      i += 1;
    } else if (arg === '--operation-key') {
      options.operationKey = argv[i + 1] || options.operationKey;
      i += 1;
    } else if (arg === '--data-mode') {
      options.dataMode = argv[i + 1] || options.dataMode;
      i += 1;
    } else if (arg === '--environment') {
      options.environment = argv[i + 1] || options.environment;
      i += 1;
    } else if (arg === '--retry') {
      options.retry = true;
    }
  }

  return options;
}

function printResult(label, result) {
  if (!result.ok) {
    console.warn(`${label}: telemetry failed but business flow continues.`, result.error || result.body || 'unknown error');
    return;
  }

  const warnings = Array.isArray(result.body?.warnings) ? result.body.warnings.length : 0;
  console.log(`${label}: ingest response ${result.status} ok=${result.body?.ok === true} planTier=${result.body?.planTier || 'n/a'} warnings=${warnings}`);
}

async function main() {
  const options = parseArgs(process.argv.slice(2));
  const apiKey = process.env.OPSMETER_API_KEY || '';
  const apiBaseUrl = process.env.OPSMETER_API_BASE_URL || 'https://api.opsmeter.io';
  const dryRun = process.env.OPSMETER_DRY_RUN === '1';

  if (!apiKey && !dryRun) {
    console.error('Missing OPSMETER_API_KEY.');
    process.exitCode = 1;
    return;
  }

  const payload = buildPayload({
    operationKey: options.operationKey,
    provider: options.provider,
    model: options.model,
    dataMode: options.dataMode,
    environment: options.environment
  });

  // Simulated business flow completion before telemetry completion.
  console.log('Business call completed.');

  const firstSend = sendTelemetry(payload, {
    apiBaseUrl,
    apiKey,
    dryRun,
    timeoutMs: 800
  });

  console.log('Telemetry dispatched (non-blocking).');
  printResult('Attempt 1', await firstSend);

  if (options.retry) {
    const retryResult = await sendTelemetry(payload, {
      apiBaseUrl,
      apiKey,
      dryRun,
      timeoutMs: 800
    });
    printResult('Retry with same externalRequestId', retryResult);
  }

  console.log(`externalRequestId=${payload.externalRequestId}`);
}

main().catch((error) => {
  console.error('Unexpected failure:', error instanceof Error ? error.message : error);
  process.exitCode = 1;
});
