$ErrorActionPreference = 'Stop'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
$directory = Join-Path $PSScriptRoot 'dist'
New-Item -ItemType Directory -Force $directory | Out-Null
$binary = Join-Path $directory 'Tests.exe'
$scanner = Join-Path $PSScriptRoot 'Scanner.cs'
$tests = Join-Path $PSScriptRoot 'Tests.cs'
& $compiler /nologo /target:exe "/out:$binary" /reference:System.Core.dll $scanner $tests
if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed' }
& $binary
if ($LASTEXITCODE -ne 0) { throw 'Tests failed' }

