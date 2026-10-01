<?php

namespace RSEV\Api;

defined('RSEV_EXEC') or die;

class RuntimeApiHost
{
    protected mixed $server = null;
    protected RuntimeEndpointController $endpoints;
    public function __construct(protected string $host = '127.0.0.1', protected int $port = 0)
    {
        $this->endpoints = new RuntimeEndpointController();
    }

    public function register(IRuntimeEndpoint $endpoint): bool { return $this->endpoints->register($endpoint); }
    public function unregister(string $route): bool { return $this->endpoints->unregister($route); }
    public function resolve(string $route): ?IRuntimeEndpoint { return $this->endpoints->resolve($route); }
    public function endpoints(): RuntimeEndpointController { return $this->endpoints; }
    public function port(): int { return $this->port; }

    public function start(): bool
    {
        if (is_resource($this->server)) return true;
        $this->server = stream_socket_server("tcp://{$this->host}:{$this->port}", $errorCode, $errorMessage);
        if (!is_resource($this->server)) throw new \RuntimeException($errorMessage, $errorCode);
        stream_set_blocking($this->server, false);
        $name = stream_socket_get_name($this->server, false);
        $this->port = (int) substr(strrchr($name, ':'), 1);
        return true;
    }

    public function stop(): void
    {
        if (is_resource($this->server)) fclose($this->server);
        $this->server = null;
    }

    public function tick(): void
    {
        if (!is_resource($this->server)) return;
        $connection = @stream_socket_accept($this->server, 0);
        if (is_resource($connection)) {
            stream_set_timeout($connection, 5);
            $raw = $this->readRequest($connection);
            fwrite($connection, $this->handleRaw($raw ?: '')->toHttp());
            fclose($connection);
        }
    }

    private function readRequest(mixed $connection): string
    {
        $raw = '';
        while (!str_contains($raw, "\r\n\r\n")) {
            $chunk = fread($connection, 8192);
            if ($chunk === false || $chunk === '') break;
            $raw .= $chunk;
            if (strlen($raw) > 1024 * 1024) break;
        }
        [$headers, $body] = array_pad(explode("\r\n\r\n", $raw, 2), 2, '');
        $length = 0;
        foreach (explode("\r\n", $headers) as $line) {
            if (str_contains($line, ':')) {
                [$key, $value] = explode(':', $line, 2);
                if (strcasecmp(trim($key), 'Content-Length') === 0) $length = (int) trim($value);
            }
        }
        while (strlen($body) < $length) {
            $chunk = fread($connection, $length - strlen($body));
            if ($chunk === false || $chunk === '') break;
            $body .= $chunk;
        }
        return $headers . "\r\n\r\n" . substr($body, 0, $length);
    }

    public function handleRaw(string $raw): EndpointResponse
    {
        [$headerText, $body] = array_pad(explode("\r\n\r\n", $raw, 2), 2, '');
        $lines = explode("\r\n", $headerText);
        $parts = explode(' ', $lines[0] ?? '', 3);
        if (count($parts) < 2) return EndpointResponse::fromText(400, 'Bad Request');
        $headers = [];
        foreach (array_slice($lines, 1) as $line) {
            if (str_contains($line, ':')) [$key, $value] = explode(':', $line, 2); else continue;
            $headers[trim($key)] = trim($value);
        }
        $route = parse_url($parts[1], PHP_URL_PATH) ?: '/';
        $endpoint = $this->resolve($route);
        if ($endpoint === null) return EndpointResponse::fromText(404, 'Not Found');
        try {
            return $endpoint->handle(new EndpointRequest($parts[0], $route, $headers, $body));
        } catch (\Throwable $error) {
            return EndpointResponse::fromText(500, $error->getMessage());
        }
    }
}
