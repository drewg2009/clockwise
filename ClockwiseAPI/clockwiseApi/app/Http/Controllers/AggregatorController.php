<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;

class AggregatorController extends Controller
{
    public static function executeAll(){
        $quoteController = new QuoteController();
        $quoteController->execute();
    }
}
