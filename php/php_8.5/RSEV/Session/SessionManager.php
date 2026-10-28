<?php

namespace RSEV\Session;

defined('RSEV_EXEC') or die;

final readonly class SessionManager implements ISessionManager
{
    public function __construct(private IAccessTokenProvider $tokenProvider, private int $accessLifetime = 3600, private int $refreshLifetime = 2592000) {}
    public function create(string $subject): SessionContext
    {
        $now = new \DateTimeImmutable('now', new \DateTimeZone('UTC'));
        return new SessionContext($subject, $this->tokenProvider->create($subject, $this->accessLifetime), new RefreshToken(rtrim(strtr(base64_encode(random_bytes(32)), '+/', '-_'), '='), $now->modify("+{$this->refreshLifetime} seconds")));
    }
    public function isValid(SessionContext $context): bool { return $this->tokenProvider->validate($context->accessToken) && !$context->refreshToken->isExpired(); }
}
