<?php

namespace RSEV\Plugin;

defined('RSEV_EXEC') or die;

final class PluginRegistry
{
    /** @var array<string, array{version: string, plugin: IGeneratorPlugin}> */
    private array $plugins = [];
    public function register(IGeneratorPlugin $plugin): void { if (isset($this->plugins[$plugin->name()])) throw new \InvalidArgumentException("Plugin '{$plugin->name()}' is already registered"); $this->plugins[$plugin->name()] = ['version' => $plugin->version(), 'plugin' => $plugin]; }
    public function all(): array { ksort($this->plugins); return $this->plugins; }
    public function generate(array $context): void { foreach ($this->all() as $entry) $entry['plugin']->generate($context); }
}
