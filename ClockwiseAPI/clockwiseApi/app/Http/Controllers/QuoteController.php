<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;


class QuoteController extends Controller implements ModuleInterface
{
    private $url = "http://feeds.feedburner.com/theysaidso/qod";

    public function execute($name, $limit, $message){
        $xmlObject = XMLParserController::get_rss_item($this->url, 1);
        return $message . $xmlObject[0]->description;
    }
}
