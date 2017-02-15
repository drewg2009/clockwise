<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;

class AggregatorController extends Controller
{
    private $finalString = "";

    public function __construct()
    {
    }

    public function executeAll(){

        $quoteController = new QuoteController();
        $redditController = new RedditController();
        $twitterController = new TwitterController();
        //$history = new HistoryController();

        /*

        $this->finalString .= $quoteController->execute(null, null, "Here is the quote of the day. ") . ". ";
        $this->finalString .= $twitterController->execute("epochSoftware",3, "Here are the top posts from ") . ". ";
        $this->finalString .= $twitterController->execute("OddFunFacts",1, "Here is the fun fact of the day from ") . ". ";
        $this->finalString .= $redditController->execute("sports",3, "Here are the top reddit posts from ") . ". ";
        */
        $this->finalString .= $twitterController->execute("yesterday_today",10, "On this day in history ");

        echo $this->finalString;
    }
}
