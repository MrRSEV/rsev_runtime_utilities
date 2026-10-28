import unittest
from datetime import timedelta

from rsev_utilities.crypto import HamacProvider, Sha256Provider
from rsev_utilities.session import JwtTokenProvider, SessionManager


class ManifestModuleTests(unittest.TestCase):
    def test_constructor(self):
        self.assertEqual(Sha256Provider().algorithm, "SHA-256")

    def test_happy_path(self):
        provider = HamacProvider(b"secret")
        self.assertTrue(provider.verify(b"payload", provider.compute(b"payload")))

    def test_error_handling(self):
        with self.assertRaises(ValueError):
            HamacProvider(b"")

    def test_edge_cases(self):
        provider = JwtTokenProvider(b"secret")
        session = SessionManager(provider, timedelta(minutes=1), timedelta(minutes=2)).create("subject")
        self.assertTrue(SessionManager(provider).is_valid(session))


if __name__ == "__main__":
    unittest.main()
