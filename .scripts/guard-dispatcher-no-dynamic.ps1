Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$dispatcherPath = Join-Path $repoRoot "src/ModularMonolith/BuildingBlocks/BuildingBlocks.Application/Dispatcher"

if (-not (Test-Path $dispatcherPath)) {
    throw "Dispatcher path not found: $dispatcherPath"
}

$matches = Get-ChildItem -Path $dispatcherPath -Filter *.cs -Recurse |
    Select-String -Pattern "\bdynamic\b"

if ($matches) {
    Write-Error "Dynamic keyword usage is not allowed in dispatcher layer."
    $matches | ForEach-Object { Write-Host "$($_.Path):$($_.LineNumber): $($_.Line.Trim())" }
    exit 1
}

Write-Host "Dispatcher dynamic usage guard passed."
