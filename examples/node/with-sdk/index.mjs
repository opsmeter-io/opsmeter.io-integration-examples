import * as opsmeter from '@opsmeter.io/node';
import OpenAI from 'openai';
import Anthropic from '@anthropic-ai/sdk';

function parseArgs(argv) {
  const options = {
    // Provider/model names are in catalog: https://opsmeter.io/docs/catalog
    provider: 'openai',
    model: 'gpt-4o-mini'
  };

  for (let i = 0; i < argv.length; i += 1) {
    const arg = argv[i];
    if (arg === '--provider') {
      options.provider = String(argv[i + 1] || options.provider).toLowerCase();
      i += 1;
    } else if (arg === '--model') {
      options.model = String(argv[i + 1] || options.model);
      i += 1;
    }
  }

  const supportedProviders = new Set(['openai', 'anthropic']);
  if (!supportedProviders.has(options.provider)) {
    throw new Error('Supported providers: openai, anthropic');
  }

  if (options.provider === 'anthropic' && options.model === 'gpt-4o-mini') {
    options.model = 'claude-3-5-sonnet-20241022';
  }

  return options;
}

opsmeter.init({
  apiKey: process.env.OPSMETER_API_KEY || '',
  workspaceId: 'ws_123',
  environment: 'prod'
});

const options = parseArgs(process.argv.slice(2));
const openai = new OpenAI({ apiKey: process.env.OPENAI_API_KEY });
const anthropic = new Anthropic({ apiKey: process.env.ANTHROPIC_API_KEY });

if (options.provider === 'anthropic') {
  const request = {
    model: options.model,
    max_tokens: 128,
    messages: [{ role: 'user', content: 'Give me a one-line support summary.' }]
  };

  const captured = await opsmeter.withContext(
    {
      userId: 'u_1',
      tenantId: 'tenant_a',
      endpoint: '/api/support',
      feature: 'support',
      promptVersion: 'v8'
    },
    async () => opsmeter.captureAnthropicMessageWithResult(
      () => anthropic.messages.create(request),
      { request, awaitTelemetryResponse: true }
    )
  );

  console.log('Provider:', captured.payload.provider);
  console.log('Provider model:', captured.providerResponse.model);
  console.log('Telemetry status:', captured.telemetry.status);
} else {
  const request = {
    model: options.model,
    messages: [{ role: 'user', content: 'Give me a one-line summary of retry-safe telemetry.' }]
  };

  const captured = await opsmeter.withContext(
    {
      userId: 'u_1',
      tenantId: 'tenant_a',
      endpoint: '/api/chat',
      feature: 'assistant',
      promptVersion: 'v12'
    },
    async () => opsmeter.captureOpenAIChatCompletionWithResult(
      () => openai.chat.completions.create(request),
      { request, awaitTelemetryResponse: true }
    )
  );

  console.log('Provider:', captured.payload.provider);
  console.log('Provider model:', captured.providerResponse.model);
  console.log('Telemetry status:', captured.telemetry.status);
}
