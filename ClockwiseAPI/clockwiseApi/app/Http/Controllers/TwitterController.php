<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;

class TwitterController extends Controller implements ModuleInterface
{

    public function execute($name,$limit,$message)
    {
        $twitterAPI = new TwitterAPI($name,$limit);
        $twitterContent = $twitterAPI->getContent(false, false, false);
        return $message . $name . $this->getString($twitterContent);
    }

    private function getString($array){
        for ($i=0;$i<sizeof($array);$i++){
            //dd($array[$i]);
            echo $array[$i]->text . " ";
        }
    }
}
