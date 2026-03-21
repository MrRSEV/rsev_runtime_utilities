<?php

namespace RSEV\Routing;

defined('RSEV_EXEC') or die;

use RSEV\Http\Request;
use RSEV\Http\Response;

class Router
{
    protected array $routes = [];

    public function get(string $path, callable $handler): void
    {
        $this->add('GET', $path, $handler);
    }

    public function post(string $path, callable $handler): void
    {
        $this->add('POST', $path, $handler);
    }

    public function add(string $method, string $path, callable $handler): void
    {
        $this->routes[] = compact('method', 'path', 'handler');
    }

    public function dispatch(Request $req, Response $res): void
    {
        foreach ($this->routes as $route) {
            if ($route['method'] === $req->method() && $route['path'] === $req->uri()) {
                $route['handler']($req, $res);
                return;
            }
        }

        $res->status(404);
        $res->text('404 Not Found');
    }
}
