import argparse
import os
from concurrent.futures import ThreadPoolExecutor

from telemetry import build_payload, send_telemetry


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Opsmeter Python ingest example")
    # Provider/model names are in catalog: https://opsmeter.io/docs/catalog
    parser.add_argument("--provider", default="openai", choices=["openai", "anthropic"])
    parser.add_argument("--model", default="gpt-4o-mini")
    parser.add_argument("--operation-key", default="order:1001")
    parser.add_argument("--data-mode", default="real")
    parser.add_argument("--environment", default="prod")
    parser.add_argument("--retry", action="store_true")
    return parser.parse_args()


def print_result(label: str, result: dict) -> None:
    if not result.get("ok"):
        print(f"{label}: telemetry failed but business flow continues -> {result.get('error') or result.get('body')}")
        return

    body = result.get("body") or {}
    warnings = body.get("warnings")
    warning_count = len(warnings) if isinstance(warnings, list) else 0
    print(
        f"{label}: ingest response {result.get('status')} "
        f"ok={body.get('ok', False)} planTier={body.get('planTier', 'n/a')} warnings={warning_count}"
    )


def run_once(payload: dict, api_key: str, api_base_url: str, dry_run: bool) -> dict:
    return send_telemetry(
        payload=payload,
        api_key=api_key,
        api_base_url=api_base_url,
        timeout_seconds=0.8,
        dry_run=dry_run,
    )


def main() -> int:
    args = parse_args()
    api_key = os.getenv("OPSMETER_API_KEY", "")
    api_base_url = os.getenv("OPSMETER_API_BASE_URL", "https://api.opsmeter.io")
    dry_run = os.getenv("OPSMETER_DRY_RUN", "0") == "1"

    if not api_key and not dry_run:
        print("Missing OPSMETER_API_KEY")
        return 1

    payload = build_payload(
        operation_key=args.operation_key,
        provider=args.provider,
        model=args.model,
        data_mode=args.data_mode,
        environment=args.environment,
    )

    print("Business call completed.")

    # Fire telemetry asynchronously so business flow is not blocked.
    with ThreadPoolExecutor(max_workers=1) as executor:
        first_future = executor.submit(run_once, payload, api_key, api_base_url, dry_run)
        print("Telemetry dispatched (non-blocking).")
        first_result = first_future.result()
        print_result("Attempt 1", first_result)

        if args.retry:
            retry_result = run_once(payload, api_key, api_base_url, dry_run)
            print_result("Retry with same externalRequestId", retry_result)

    print(f"externalRequestId={payload['externalRequestId']}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
