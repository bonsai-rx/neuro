[CmdletBinding()] param (
    [string[]]$docfxArgs
)
Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $true

Push-Location $PSScriptRoot
try {
    $libPaths = @()
    ./export-images.ps1 $libPaths
    dotnet docfx build $docfxArgs
} finally {
    Pop-Location
}
