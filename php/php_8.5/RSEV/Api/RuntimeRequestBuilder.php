<?php

namespace RSEV\Api;

defined('RSEV_EXEC') or die;

class RuntimeRequestBuilder
{
    private string $route = '/';
    private mixed $payload = null;
    private array $headers = [];
    private ?string $sessionId = null;
    public function withRoute(string $route): self { $this->route = $route; return $this; }
    public function withPayload(mixed $payload): self { $this->payload = $payload; return $this; }
    public function withHeader(string $name, string $value): self { $this->headers[$name] = $value; return $this; }
    public function bindSession(?RuntimeSession $session): self { $this->sessionId = $session?->sessionId; return $this; }
    public function build(): RuntimeRequest { return new RuntimeRequest($this->route, $this->payload, $this->headers, $this->sessionId); }
    public function buildBytes(): string { $request = $this->build(); $body = $request->payload === null ? '' : json_encode($request->payload, JSON_THROW_ON_ERROR); $headers = ['Content-Length' => (string) strlen($body), 'Content-Type' => 'application/json', ...$request->headers]; if ($request->sessionId) $headers['X-Session-Id'] = $request->sessionId; $result = "POST {$request->route} HTTP/1.1\r\n"; foreach ($headers as $key => $value) $result .= "$key: $value\r\n"; return $result . "\r\n" . $body; }
}
