import argparse
import os

import opsmeter_sdk as opsmeter
from anthropic import Anthropic
from openai import OpenAI


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Opsmeter Python SDK sample")
    # Provider/model names are in catalog: https://opsmeter.io/docs/catalog
    parser.add_argument("--provider", default="openai", choices=["openai", "anthropic"])
    parser.add_argument("--model", default="gpt-4o-mini")
    args = parser.parse_args()

    if args.provider == "anthropic" and args.model == "gpt-4o-mini":
        args.model = "claude-3-5-sonnet-20241022"
    return args


def main() -> int:
    args = parse_args()

    opsmeter.init(
        api_key=os.getenv("OPSMETER_API_KEY", ""),
        workspace_id="ws_123",
        environment="prod",
    )

    if args.provider == "anthropic":
        client = Anthropic(api_key=os.getenv("ANTHROPIC_API_KEY", ""))
        with opsmeter.context(
            user_id="u_1",
            tenant_id="tenant_a",
            endpoint="/api/support",
            feature="support",
            prompt_version="v8",
        ):
            captured = opsmeter.capture_anthropic_message_with_result(
                lambda: client.messages.create(
                    model=args.model,
                    max_tokens=128,
                    messages=[{"role": "user", "content": "Give one support summary line."}],
                ),
                request={"model": args.model},
                await_telemetry_response=True,
            )
    else:
        client = OpenAI(api_key=os.getenv("OPENAI_API_KEY", ""))
        with opsmeter.context(
            user_id="u_1",
            tenant_id="tenant_a",
            endpoint="/api/chat",
            feature="assistant",
            prompt_version="v12",
        ):
            captured = opsmeter.capture_openai_chat_completion_with_result(
                lambda: client.chat.completions.create(
                    model=args.model,
                    messages=[{"role": "user", "content": "Give one sentence about telemetry attribution."}],
                ),
                request={"model": args.model},
                await_telemetry_response=True,
            )

    print("Provider:", captured["payload"]["provider"])
    print("Provider model:", captured["provider_response"].model)
    print("Telemetry status:", captured["telemetry"]["status"])
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
