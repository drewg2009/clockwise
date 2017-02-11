<?php
/**
 * Created by PhpStorm.
 * User: drewgallagher
 * Date: 2/10/17
 * Time: 9:39 PM
 */

namespace App\Http\Controllers;


interface ModuleInterface
{
    public function execute($name,$limit,$message);

}