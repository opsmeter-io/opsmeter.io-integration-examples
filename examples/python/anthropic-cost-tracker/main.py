import os
import sys
import uuid
from concurrent.futures import ThreadPoolExecutor
from datetime import datetime
from pathlib import Path
from time import perf_counter
from typing import Any

from anthropic import Anthropic
from dotenv import load_dotenv

# Allow direct script execution while reusing the shared Python telemetry helper.
sys.path.append(str(Path(__file__).resolve().parents[1]))

from telemetry import send_telemetry  # type: ignore  # noqa: E402


load_dotenv(Path(__file__).with_name(".env"))


def require_env(name: str) -> str:
    value = os.getenv(name, "").strip()
    if not value:
        raise ValueError(f"Missing required environment variable: {name}")
    return value


def usage_value(usage: Any, key: str) -> int:
    if usage is None:
        return 0
    if isinstance(usage, dict):
        value = usage.get(key, 0)
        return value if isinstance(value, int) else 0

    value = getattr(usage, key, 0)
    return value if isinstance(value, int) else 0


def extract_text(content: Any) -> str:
    if not isinstance(content, list):
        return ""

    chunks: list[str] = []
    for block in content:
        block_type = getattr(block, "type", None)
        text_value = getattr(block, "text", None)

        if isinstance(block, dict):
            block_type = block.get("type")
            text_value = block.get("text")

        if block_type == "text" and isinstance(text_value, str):
            chunks.append(text_value)

    return "\n".join(chunks).strip()


def build_base_payload(external_request_id: str, latency_ms: int) -> dict[str, Any]:
    payload: dict[str, Any] = {
        "externalRequestId": external_request_id,
        "provider": "anthropic",
        "model": os.getenv("ANTHROPIC_MODEL", "claude-3-5-sonnet-20241022"),
        "promptVersion": os.getenv("OPSMETER_PROMPT_VERSION", "support_reply_v2"),
        "endpointTag": os.getenv("OPSMETER_ENDPOINT_TAG", "support.reply"),
        "latencyMs": latency_ms,
        "dataMode": os.getenv("OPSMETER_DATA_MODE", "real"),
        "environment": os.getenv("OPSMETER_ENVIRONMENT", "prod"),
        "createdAt": datetime.utcnow().isoformat() + "Z",
    }

    tenant_id = os.getenv("OPSMETER_TENANT_ID", "").strip()
    feature_tag = os.getenv("OPSMETER_FEATURE_TAG", "").strip()

    if tenant_id:
        payload["tenantId"] = tenant_id
    if feature_tag:
        payload["featureTag"] = feature_tag

    return payload


def print_usage(input_tokens: int, output_tokens: int, total_tokens: int) -> None:
    print("\nUsage:")
    print(f"  inputTokens: {input_tokens}")
    print(f"  outputTokens: {output_tokens}")
    print(f"  totalTokens: {total_tokens}")


def ingest_payload(payload: dict[str, Any]) -> dict[str, Any]:
    return send_telemetry(
        payload=payload,
        api_key=require_env("OPSMETER_API_KEY"),
        api_base_url=os.getenv("OPSMETER_API_BASE_URL", os.getenv("OPSMETER_BASE_URL", "https://api.opsmeter.io")),
        timeout_seconds=1.5,
        dry_run=os.getenv("OPSMETER_DRY_RUN", "0") == "1",
    )


def main() -> int:
    require_env("ANTHROPIC_API_KEY")
    require_env("OPSMETER_API_KEY")

    client = Anthropic(api_key=os.getenv("ANTHROPIC_API_KEY"))
    external_request_id = str(uuid.uuid4())
    started_at = perf_counter()

    try:
        response = client.messages.create(
            model=os.getenv("ANTHROPIC_MODEL", "claude-3-5-sonnet-20241022"),
            max_tokens=256,
            messages=[
                {
                    "role": "user",
                    "content": os.getenv(
                        "EXAMPLE_INPUT",
                        "Summarize why provider-agnostic telemetry matters for AI cost control in three bullets.",
                    ),
                }
            ],
        )

        latency_ms = int((perf_counter() - started_at) * 1000)
        usage = getattr(response, "usage", None)
        input_tokens = usage_value(usage, "input_tokens")
        output_tokens = usage_value(usage, "output_tokens")
        total_tokens = input_tokens + output_tokens

        payload = build_base_payload(external_request_id, latency_ms)
        payload.update(
            {
                "model": getattr(response, "model", None) or os.getenv("ANTHROPIC_MODEL", "claude-3-5-sonnet-20241022"),
                "inputTokens": input_tokens,
                "outputTokens": output_tokens,
                "totalTokens": total_tokens,
                "status": "success",
                "errorCode": None,
            }
        )

        with ThreadPoolExecutor(max_workers=1) as executor:
            future = executor.submit(ingest_payload, payload)
            ingest_response = future.result()

        print("\nModel output:\n")
        print(extract_text(getattr(response, "content", None)) or "(no text content returned)")
        print_usage(input_tokens, output_tokens, total_tokens)
        print("\nOpsmeter payload:")
        print(payload)
        print("\nIngest response:")
        print(ingest_response)
        return 0
    except Exception as error:
        latency_ms = int((perf_counter() - started_at) * 1000)
        error_payload = build_base_payload(external_request_id, latency_ms)
        error_payload.update(
            {
                "inputTokens": 0,
                "outputTokens": 0,
                "totalTokens": 0,
                "status": "error",
                "errorCode": type(error).__name__,
            }
        )

        try:
            ingest_payload(error_payload)
        except Exception:
            pass

        print("\nExample failed:")
        print(str(error))
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
