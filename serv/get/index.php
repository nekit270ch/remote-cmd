<?php
$data = $_GET['data'];
$id = $_GET['id'];

echo file_get_contents("../usr/$id/$data.txt");