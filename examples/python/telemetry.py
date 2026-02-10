import hashlib
import json
import urllib.error
import urllib.request
from typing import Any, Dict, Optional

VALID_DATA_MODES = {"real", "test", "demo"}
VALID_ENVIRONMENTS = {"prod", "staging", "dev"}


def build_external_request_id(operation_key: str) -> str:
    normalized = (operation_key or "").strip().lower()
    if not normalized:
        raise ValueError("operation_key is required")

    digest = hashlib.sha256(normalized.encode("utf-8")).hexdigest()[:24]
    return f"ext_{digest}"


def _normalize_data_mode(value: Optional[str]) -> str:
    normalized = (value or "real").strip().lower()
    if normalized not in VALID_DATA_MODES:
        raise ValueError("dataMode must be real|test|demo")
    return normalized


def _normalize_environment(value: Optional[str]) -> str:
    normalized = (value or "prod").strip().lower()
    if normalized not in VALID_ENVIRONMENTS:
        raise ValueError("environment must be prod|staging|dev")
    return normalized


def build_payload(
    operation_key: str,
    provider: str,
    model: str,
    data_mode: str = "real",
    environment: str = "prod",
    user_id: Optional[str] = None,
) -> Dict[str, Any]:
    provider = (provider or "").strip()
    model = (model or "").strip()
    if not provider or not model:
        raise ValueError("provider and model are required")

    input_tokens = 120
    output_tokens = 45
    is_anthropic = provider.lower() == "anthropic"

    return {
        "externalRequestId": build_external_request_id(operation_key),
        "provider": provider,
        "model": model,
        "promptVersion": "support_reply_v2" if is_anthropic else "summary_v3",
        "endpointTag": "support.reply" if is_anthropic else "checkout.ai_summary",
        "inputTokens": input_tokens,
        "outputTokens": output_tokens,
        "totalTokens": input_tokens + output_tokens,
        "latencyMs": 820,
        "status": "success",
        "errorCode": None,
        "userId": user_id,
        "dataMode": _normalize_data_mode(data_mode),
        "environment": _normalize_environment(environment),
    }


def send_telemetry(
    payload: Dict[str, Any],
    api_key: str,
    api_base_url: str = "https://api.opsmeter.io",
    timeout_seconds: float = 0.8,
    dry_run: bool = False,
) -> Dict[str, Any]:
    if dry_run:
        return {
            "ok": True,
            "status": 200,
            "body": {
                "ok": True,
                "planTier": "Free",
                "warnings": ["dry-run mode: request not sent"],
            },
        }

    endpoint = f"{api_base_url.rstrip('/')}/v1/ingest/llm-request"
    request = urllib.request.Request(
        endpoint,
        data=json.dumps(payload).encode("utf-8"),
        headers={
            "Content-Type": "application/json",
            "X-API-Key": api_key,
        },
        method="POST",
    )

    try:
        with urllib.request.urlopen(request, timeout=timeout_seconds) as response:
            raw = response.read().decode("utf-8")
            body = json.loads(raw) if raw else {}
            return {
                "ok": 200 <= response.status < 300,
                "status": response.status,
                "body": body,
            }
    except urllib.error.HTTPError as error:
        body_text = error.read().decode("utf-8") if error.fp else ""
        try:
            body = json.loads(body_text) if body_text else {}
        except json.JSONDecodeError:
            body = {"raw": body_text}
        return {"ok": False, "status": error.code, "body": body}
    except Exception as error:  # swallow to keep business flow alive
        return {"ok": False, "status": 0, "error": str(error)}
