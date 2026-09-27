param([string]$OutputDirectory = "$PSScriptRoot/dist")
$ErrorActionPreference = 'Stop'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
New-Item -ItemType Directory -Force $OutputDirectory | Out-Null
$binary = Join-Path $OutputDirectory 'UnityLogFinder.exe'
$scanner = Join-Path $PSScriptRoot 'Scanner.cs'
$program = Join-Path $PSScriptRoot 'Program.cs'
$icon = Join-Path $PSScriptRoot 'UnityLogFinder.ico'
& $compiler /nologo /target:winexe /platform:anycpu /optimize+ "/win32icon:$icon" "/out:$binary" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:System.Core.dll $scanner $program
if ($LASTEXITCODE -ne 0) { throw 'Build failed' }
Write-Host "Built $OutputDirectory/UnityLogFinder.exe"


