<?php

namespace RSEV\Api;

defined('RSEV_EXEC') or die;

class EndpointResponse
{
    public function __construct(
        public readonly int $statusCode = 200,
        public readonly string $body = '',
        public readonly array $headers = [],
    ) {
    }

    public static function fromText(int $statusCode, string $text): self
    {
        return new self($statusCode, $text, ['Content-Type' => 'text/plain; charset=utf-8']);
    }

    public static function fromJson(mixed $value, int $statusCode = 200): self
    {
        return new self($statusCode, json_encode($value, JSON_THROW_ON_ERROR), ['Content-Type' => 'application/json; charset=utf-8']);
    }

    public function toHttp(): string
    {
        $headers = ['Content-Length' => (string) strlen($this->body), 'Connection' => 'close', ...$this->headers];
        $result = 'HTTP/1.1 ' . $this->statusCode . ' ' . self::reason($this->statusCode) . "\r\n";
        foreach ($headers as $key => $value) $result .= $key . ': ' . $value . "\r\n";
        return $result . "\r\n" . $this->body;
    }

    private static function reason(int $statusCode): string
    {
        return match ($statusCode) {
            200 => 'OK', 201 => 'Created', 400 => 'Bad Request', 404 => 'Not Found', default => $statusCode >= 500 ? 'Internal Server Error' : 'Response',
        };
    }
}
