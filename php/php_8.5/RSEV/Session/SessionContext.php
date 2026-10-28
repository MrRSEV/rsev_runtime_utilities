<?php

namespace RSEV\Session;

defined('RSEV_EXEC') or die;

final readonly class SessionContext
{
    public function __construct(public string $subject, public AccessToken $accessToken, public RefreshToken $refreshToken) {}
}
