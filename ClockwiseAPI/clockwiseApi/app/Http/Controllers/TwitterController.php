<?php

namespace App\Http\Controllers;

class TwitterController extends Controller implements ModuleInterface
{

    public function execute($name, $limit, $message)
    {
        $string = "";
        $twitterAPI = new TwitterAPI($name, $limit);
        $twitterContent = $twitterAPI->getContent(false, false, false);

        if (sizeof($twitterContent) > 0) {
            $string = $message . $name . $this->getString($twitterContent);
        } else {
            $string = "Could not pull from twitter";
        }
        return $string;
    }

    private function getString($array)
    {
        $string = "";
        for ($i = 0; $i < sizeof($array); $i++) {
            $string .= $array[$i]->text . " ";
        }
        return $string;
    }
}
