<?php
class XMLParser
{
    public static function get_atom_entries($url, $limit){
    	$entries = array();
    	$xml=simplexml_load_file($url) or return $entries;
		for ($x = 0; $x < $limit; $x++) {
			$entries[x] = $xml->entry[x]->title;
		}
		return $entries;
    }

    public static function get_rss_item($url, $limit){
    	$items = array();
    	$xml=simplexml_load_file($url) or return $items;
		for ($x = 0; $x < $limit; $x++) {
			$items[x] = $xml->item[x]->title;
		}
		return $items;
    }
}
?>
