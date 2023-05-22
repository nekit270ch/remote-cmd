<?php
$data = $_GET['data'];
$txt = $_GET['txt'];
$id = $_GET['id'];

file_put_contents("../usr/$id/$data.txt", $txt);