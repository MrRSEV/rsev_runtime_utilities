<?php

namespace RSEV\Plugin;

defined('RSEV_EXEC') or die;

class PluginLoader
{
    protected array $plugins = [];

    public function load(array $classes): void
    {
        foreach ($classes as $class) {
            if (class_exists($class)) {
                $plugin = new $class();
                if ($plugin instanceof PluginInterface) {
                    $plugin->boot();
                    $this->plugins[] = $plugin;
                }
            }
        }
    }
}
