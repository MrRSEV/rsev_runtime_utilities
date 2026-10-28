<?php

namespace RSEV\Api;

defined('RSEV_EXEC') or die;

final class HttpApiClient implements IApiClient
{
    public function __construct(private readonly RetryPolicy $retryPolicy = new RetryPolicy(), private readonly RateLimitPolicy $rateLimitPolicy = new RateLimitPolicy()) {}
    public function send(ApiRequest $request): ApiResponse
    {
        for ($attempt = 1; $attempt <= $this->retryPolicy->maxAttempts; $attempt++) {
            if (!$this->rateLimitPolicy->tryAcquire()) throw new \RuntimeException('API rate limit reached');
            $context = stream_context_create(['http' => ['method' => $request->method, 'header' => $this->headers($request->headers), 'content' => $request->body, 'ignore_errors' => true, 'timeout' => 30]]);
            $body = @file_get_contents($request->url, false, $context);
            $headers = $http_response_header ?? [];
            $status = (int) (preg_match('/^HTTP\/\S+\s+(\d+)/', $headers[0] ?? '', $match) ? $match[1] : 500);
            $result = new ApiResponse($status, $body === false ? '' : $body, $this->parseHeaders($headers));
            if ($status < 500 || $attempt === $this->retryPolicy->maxAttempts) return $result;
            usleep($this->retryPolicy->delayMilliseconds * 1000);
        }
        throw new \RuntimeException('request retry loop ended unexpectedly');
    }
    private function headers(array $headers): string { return implode("\r\n", array_map(static fn (string $key, string $value): string => "$key: $value", array_keys($headers), $headers)); }
    private function parseHeaders(array $headers): array { $result = []; foreach (array_slice($headers, 1) as $line) { if (str_contains($line, ':')) { [$key, $value] = explode(':', $line, 2); $result[trim($key)] = trim($value); } } return $result; }
}
