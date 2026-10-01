<?php

namespace RSEV\Api;

defined('RSEV_EXEC') or die;

class RuntimeApiClient
{
    private ?RuntimeSession $session = null;
    public function __construct(private readonly string $host, private readonly int $port, private readonly bool $useTls = false) {}
    public function setSession(?RuntimeSession $session): self { $this->session = $session; return $this; }
    public function call(string $route, mixed $payload = null): RuntimeResponse { return $this->send((new RuntimeRequestBuilder())->withRoute($route)->withPayload($payload)->build()); }
    public function send(RuntimeRequest $request): RuntimeResponse
    {
        $scheme = $this->useTls ? 'tls' : 'tcp';
        $connection = @stream_socket_client("{$scheme}://{$this->host}:{$this->port}", $errorCode, $errorMessage, 30);
        if (!is_resource($connection)) throw new \RuntimeException($errorMessage, $errorCode);
        $headers = $request->headers;
        $body = $request->payload === null ? '' : json_encode($request->payload, JSON_THROW_ON_ERROR);
        $headers['Content-Length'] = (string) strlen($body);
        if ($body !== '') $headers['Content-Type'] ??= 'application/json';
        if ($this->session) $headers['X-Session-Id'] = $this->session->sessionId;
        $raw = "POST {$request->route} HTTP/1.1\r\n";
        foreach ($headers as $key => $value) $raw .= "$key: $value\r\n";
        fwrite($connection, $raw . "\r\n" . $body);
        $response = stream_get_contents($connection) ?: '';
        fclose($connection);
        [$headerText, $responseBody] = array_pad(explode("\r\n\r\n", $response, 2), 2, '');
        $status = (int) (explode(' ', explode("\r\n", $headerText)[0] ?? ' 500 ', 3)[1] ?? 500);
        $responseHeaders = [];
        foreach (array_slice(explode("\r\n", $headerText), 1) as $line) {
            if (!str_contains($line, ':')) continue;
            [$key, $value] = explode(':', $line, 2);
            $responseHeaders[trim($key)] = trim($value);
        }
        return new RuntimeResponse($status, $responseBody, $responseHeaders, $status >= 400 ? $responseBody : null);
    }
}
