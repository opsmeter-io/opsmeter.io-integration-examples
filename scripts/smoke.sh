#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

export OPSMETER_DRY_RUN="${OPSMETER_DRY_RUN:-1}"
export OPSMETER_API_KEY="${OPSMETER_API_KEY:-dummy-key}"
export OPSMETER_API_BASE_URL="${OPSMETER_API_BASE_URL:-https://api.opsmeter.io}"

echo "[smoke] node example"
node "${ROOT_DIR}/examples/node/index.mjs" --provider openai --model gpt-4o-mini --retry >/tmp/opsmeter-node-smoke.log
cat /tmp/opsmeter-node-smoke.log

echo "[smoke] python example"
python3 "${ROOT_DIR}/examples/python/main.py" --provider anthropic --model claude-3-5-sonnet-20241022 --data-mode test --environment staging --retry >/tmp/opsmeter-python-smoke.log
cat /tmp/opsmeter-python-smoke.log

echo "[smoke] dotnet example"
dotnet run --project "${ROOT_DIR}/examples/dotnet/src/Opsmeter.IntegrationExamples" -- --provider openai --model gpt-4o-mini --retry >/tmp/opsmeter-dotnet-smoke.log
cat /tmp/opsmeter-dotnet-smoke.log

echo "[smoke] all examples passed"
