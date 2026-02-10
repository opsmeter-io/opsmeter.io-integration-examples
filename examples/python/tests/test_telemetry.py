import unittest

from examples.python.telemetry import build_external_request_id, build_payload


class TelemetryTests(unittest.TestCase):
    def test_external_request_id_is_deterministic(self):
        first = build_external_request_id("order:1001")
        second = build_external_request_id("order:1001")
        self.assertEqual(first, second)
        self.assertTrue(first.startswith("ext_"))

    def test_payload_retry_uses_same_external_request_id(self):
        first = build_payload(
            operation_key="invoice:42",
            provider="openai",
            model="gpt-4o-mini",
            data_mode="real",
            environment="prod",
        )
        retry = build_payload(
            operation_key="invoice:42",
            provider="openai",
            model="gpt-4o-mini",
            data_mode="real",
            environment="prod",
        )

        self.assertEqual(first["externalRequestId"], retry["externalRequestId"])

    def test_payload_supports_anthropic_test_mode(self):
        payload = build_payload(
            operation_key="support:77",
            provider="anthropic",
            model="claude-3-5-sonnet-20241022",
            data_mode="test",
            environment="staging",
        )

        self.assertEqual(payload["provider"], "anthropic")
        self.assertEqual(payload["dataMode"], "test")
        self.assertEqual(payload["environment"], "staging")


if __name__ == "__main__":
    unittest.main()
