<?php

namespace RSEV\Plugin;

defined('RSEV_EXEC') or die;

interface IGeneratorPlugin
{
    public function name(): string;
    public function version(): string;
    public function generate(array $context): void;
}
