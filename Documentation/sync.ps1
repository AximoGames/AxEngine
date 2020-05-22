$scriptpath = $MyInvocation.MyCommand.Path
$dir = Split-Path $scriptpath

Push-Location $dir

$sourceRoot = "_site"
$destinationRoot = "..\..\AximoGames-pages\docs\"

Remove-Item -Recurse -Force $destinationRoot
Copy-Item -Path $sourceRoot -Recurse -Destination $destinationRoot -Container

$sourceRoot = "..\..\AximoGames-pages\docs-base\*"
$destinationRoot = "..\..\AximoGames-pages\docs\"

Copy-Item -Path $sourceRoot -Recurse -Destination $destinationRoot -Container

[RegEx]$Search = '(index\.html)"'
$Replace = '"'

[RegEx]$Search2 = '(\.html)"'
$Replace2 = '"'

ForEach ($File in (Get-ChildItem -Path $destinationRoot -Recurse -File)) {
    (Get-Content $File.FullName) -Replace $Search,$Replace -Replace $Search2,$Replace2 | 
        Set-Content $File.FullName
}

Pop-Location