<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;

class HistoryController extends Controller implements ModuleInterface
{
    private  $url = "http://www.infoplease.com/rss/dayinhistory.rss";

    public function execute($name, $limit, $message)
    {
        $xmlObject = XMLParserController::get_rss_item($this->url, $limit);

    }

}
