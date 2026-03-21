<?php

namespace RSEV\Runtime;

defined('RSEV_EXEC') or die;

class ExitCodes
{
    public const SUCCESS = 0;
    public const ERROR = 1;
    public const CONFIG_MISSING = 10;
    public const DB_ERROR = 20;
}
