<?php
$id = ((int)file_get_contents('../maxId.txt'))+1;

mkdir("../usr/$id");
file_put_contents("../usr/$id/cmd.txt", 'null');
file_put_contents("../usr/$id/result.txt", 'null');
file_put_contents('../maxId.txt', $id);

echo $id;