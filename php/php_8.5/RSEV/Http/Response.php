<?php

namespace RSEV\Http;

defined('RSEV_EXEC') or die;

class Response
{
    public function status(int $code): void
    {
        http_response_code($code);
    }

    public function json(array $data): void
    {
        header('Content-Type: application/json');
        echo json_encode($data);
    }

    public function text(string $content): void
    {
        echo $content;
    }
}
