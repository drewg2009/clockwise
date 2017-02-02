<?php
class XMLParser
{
    public static function get_atom_entries($input, $limit){
    	$entries = array();
    	$xml=simplexml_load_file(input) or return $entries;
		for ($x = 0; $x < limit; $x++) {
		    $entries[x] = $xml->entry[x]->title;
		}
		return $entries;
    }

    public static function get_rss_item($input, $limit){
    	$items = array();
    	$xml=simplexml_load_file(input) or return $items;
		for ($x = 0; $x < limit; $x++) {
		    $items[x] = $xml->item[x]->title;
		}
		return $items;
    }
}
?>