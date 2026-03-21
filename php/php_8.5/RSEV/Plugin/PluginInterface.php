<?php

namespace RSEV\Plugin;

defined('RSEV_EXEC') or die;

interface PluginInterface
{
    public function boot(): void;
}
