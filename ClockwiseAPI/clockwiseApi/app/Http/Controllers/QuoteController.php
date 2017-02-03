<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;


class QuoteController extends Controller
{
    private $url = "http://feeds.feedburner.com/theysaidso/qod";

    public function execute(){
        $xmlObject = XMLParserController::get_rss_item($this->url, 1);
        echo $xmlObject[0];
    }
}
