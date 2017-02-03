<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;

class XMLParserController extends Controller
{
    public static function get_atom_entries($url, $limit){
        $entries = array();
        $xml=simplexml_load_file($url);
        for ($x = 0; $x < $limit; $x++) {
            $items[$x] = $xml->channel->item[0]->description;
        }

        return $entries;
    }

    public static function get_rss_item($url, $limit){
        $items = array();
        $xml=simplexml_load_file($url);
        for ($x = 0; $x < $limit; $x++) {
            $items[$x] = $xml->channel->item[0]->description;
        }

        return $items;
    }
}
