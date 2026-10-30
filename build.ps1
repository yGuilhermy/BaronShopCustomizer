param(
    [string]$SdkDir = $env:MOD_SDK_DIR,
    [switch]$IncludeLocalAudio
)

$ErrorActionPreference = 'Stop'
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw 'Instale o SDK .NET 8 ou superior: https://dotnet.microsoft.com/download'
}
$compatibleSdk = @(& dotnet --list-sdks | Where-Object { $_ -match '^(\d+)\.' -and [int]$Matches[1] -ge 8 })
if ($compatibleSdk.Count -eq 0) {
    throw 'Instale o SDK .NET 8 ou superior: https://dotnet.microsoft.com/download'
}
if (-not $SdkDir) {
    $SdkDir = Join-Path $PSScriptRoot 'sdk'
}
if (-not (Test-Path -LiteralPath $SdkDir -PathType Container)) {
    throw 'Informe -SdkDir com as bibliotecas de referencia do jogo ou SDK compativel.'
}
$SdkDir = (Resolve-Path -LiteralPath $SdkDir).Path
$project = Join-Path $PSScriptRoot 'BaronShopCustomizer.csproj'
& dotnet build $project -c Release "-p:SdkDir=$SdkDir"
if ($LASTEXITCODE -ne 0) { throw 'A compilacao falhou.' }
& dotnet run --project (Join-Path $PSScriptRoot 'tests/LogicTests.csproj') -c Release
if ($LASTEXITCODE -ne 0) { throw 'Os testes falharam.' }
$package = Join-Path $PSScriptRoot 'dist/BaronShopCustomizer'
New-Item -ItemType Directory -Path $package -Force | Out-Null
try {
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'bin/Release/netstandard2.1/BaronShopCustomizer.dll') -Destination $package -Force
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'manifest.json') -Destination $package -Force
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'LICENSE') -Destination $package -Force
} catch {
    throw "Build e testes concluidos, mas nao foi possivel empacotar. Feche processos que estejam usando dist e tente novamente. $($_.Exception.Message)"
}
if ($IncludeLocalAudio) { throw 'Este mod nao utiliza audio externo.' }
Write-Host "Build e testes concluidos. Pacote: $package"
