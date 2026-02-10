import test from 'node:test';
import assert from 'node:assert/strict';
import { buildExternalRequestId, buildPayload } from '../telemetry.mjs';

test('buildExternalRequestId is deterministic', () => {
  const first = buildExternalRequestId('order:1001');
  const second = buildExternalRequestId('order:1001');
  assert.equal(first, second);
  assert.ok(first.startsWith('ext_'));
});

test('payload uses same externalRequestId across retries for same operation key', () => {
  const first = buildPayload({
    operationKey: 'invoice:42',
    provider: 'openai',
    model: 'gpt-4o-mini',
    dataMode: 'real',
    environment: 'prod'
  });

  const retry = buildPayload({
    operationKey: 'invoice:42',
    provider: 'openai',
    model: 'gpt-4o-mini',
    dataMode: 'real',
    environment: 'prod'
  });

  assert.equal(first.externalRequestId, retry.externalRequestId);
});

test('payload supports anthropic + test/staging mode', () => {
  const payload = buildPayload({
    operationKey: 'support:88',
    provider: 'anthropic',
    model: 'claude-3-5-sonnet-20241022',
    dataMode: 'test',
    environment: 'staging'
  });

  assert.equal(payload.provider, 'anthropic');
  assert.equal(payload.dataMode, 'test');
  assert.equal(payload.environment, 'staging');
  assert.equal(payload.status, 'success');
});
