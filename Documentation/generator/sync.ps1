$scriptpath = $MyInvocation.MyCommand.Path
$dir = Split-Path $scriptpath

cd $dir

$sourceRoot = "_site"
$destinationRoot = "..\..\..\AximoGames-pages\docs\"

Remove-Item -Recurse -Force $destinationRoot
Copy-Item -Path $sourceRoot -Recurse -Destination $destinationRoot -Container

$sourceRoot = "..\..\..\AximoGames-pages\docs-base\*"
$destinationRoot = "..\..\..\AximoGames-pages\docs\"

Copy-Item -Path $sourceRoot -Recurse -Destination $destinationRoot -Container
