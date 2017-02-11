<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;

class RedditController extends Controller implements ModuleInterface
{
    private $url = "https://www.reddit.com/r/";
    private $ext = ".rss";
    private $redditFeed;

    public function __construct($feed)
    {
        $this->redditFeed = $feed;
    }

    public function execute($name,$limit,$message)
    {
        $xmlObject = XMLParserController::get_rss_item($this->url . $this->redditFeed . $this->ext, 1);

        return $message . $xmlObject[0]->description;
    }
}
