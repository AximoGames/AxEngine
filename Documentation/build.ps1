$scriptpath = $MyInvocation.MyCommand.Path
$dir = Split-Path $scriptpath

Push-Location $dir

Remove-Item -Recurse -Force _site
docfx

Pop-Location