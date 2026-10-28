<?php

namespace RSEV\Session;

defined('RSEV_EXEC') or die;

final readonly class JwtTokenProvider implements IAccessTokenProvider
{
    public function __construct(private string $secret) { if ($secret === '') throw new \InvalidArgumentException('secret is required'); }
    public function create(string $subject, int $lifetimeSeconds): AccessToken
    {
        if ($subject === '' || $lifetimeSeconds < 1) throw new \InvalidArgumentException('subject and positive lifetime are required');
        $expires = new \DateTimeImmutable('now', new \DateTimeZone('UTC')); $expires = $expires->modify("+$lifetimeSeconds seconds");
        $header = $this->encode(['alg' => 'HS256', 'typ' => 'JWT']);
        $payload = $this->encode(['sub' => $subject, 'exp' => $expires->getTimestamp()]);
        $unsigned = "$header.$payload";
        return new AccessToken($unsigned . '.' . $this->encode(hash_hmac('sha256', $unsigned, $this->secret, true)), $expires);
    }
    public function validate(AccessToken $token): bool
    {
        if ($token->isExpired()) return false;
        $parts = explode('.', $token->value); if (count($parts) !== 3) return false;
        $expected = hash_hmac('sha256', "$parts[0].$parts[1]", $this->secret, true);
        return hash_equals($expected, $this->decode($parts[2]));
    }
    private function encode(array|string $value): string { return rtrim(strtr(base64_encode(is_array($value) ? json_encode($value, JSON_THROW_ON_ERROR) : $value), '+/', '-_'), '='); }
    private function decode(string $value): string { return base64_decode(strtr($value . str_repeat('=', (4 - strlen($value) % 4) % 4), '-_', '+/'), true) ?: ''; }
}
