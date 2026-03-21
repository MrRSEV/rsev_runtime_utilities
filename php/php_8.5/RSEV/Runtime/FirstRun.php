<?php

namespace RSEV\Runtime;

defined('RSEV_EXEC') or die;

class FirstRun
{
    public static function ensure(string $configPath): void
    {
        if (!file_exists($configPath)) {
            $directory = dirname($configPath);
            if (!is_dir($directory)) {
                mkdir($directory, 0777, true);
            }
            file_put_contents($configPath, "<?php return [];");
        }
    }
}
