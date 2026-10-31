<#
.SYNOPSIS
    Ejecuta las pruebas E2E en Android (Appium), genera el reporte Allure y compila la documentación DocFX.
.DESCRIPTION
    1. Comprueba si el emulador Android está en ejecución; de lo contrario, lo inicia y espera a que esté listo.
    2. Comprueba si el servidor de Appium está activo en el puerto 4723; de lo contrario, lo inicia en segundo plano.
    3. Compila la aplicación Android (.NET MAUI) en Release.
    4. Ejecuta las pruebas E2E con NUnit y Allure.
    5. Genera el reporte visual Allure HTML en docs/allure-report.
    6. Compila el sitio estático de DocFX integrando el reporte Allure y Cobertura.
    7. Abre el reporte Allure o DocFX en el navegador (salvo que se indique -NoBrowser).
#>

[CmdletBinding()]
param(
    [switch]$NoBrowser,
    [string]$AvdName = "seypos_scanmax_22d",
    [string]$AppiumPort = "4723"
)

$ErrorActionPreference = "Stop"

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "  TestGithub - Pruebas E2E Android (Appium + Allure)      " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

$rootDir = Split-Path -Parent $PSScriptRoot
Set-Location $rootDir

# 1. Configurar variables de entorno y herramientas locales
Write-Host "`n[1/6] Comprobando entorno y herramientas..." -ForegroundColor Yellow
$sdkRoot = if ($env:ANDROID_HOME) { $env:ANDROID_HOME } else { "E:\Android_SDK" }
$adbPath = Join-Path $sdkRoot "platform-tools\adb.exe"
$emulatorPath = Join-Path $sdkRoot "emulator\emulator.exe"

if (-not (Test-Path $adbPath)) {
    throw "No se encontró adb.exe en $adbPath. Verifique la variable ANDROID_HOME."
}

# 2. Comprobar / Iniciar Emulador Android
Write-Host "`n[2/6] Verificando emulador Android ($AvdName)..." -ForegroundColor Yellow
$devices = & $adbPath devices | Select-String "device$"
if (-not $devices) {
    Write-Host "Iniciando emulador Android '$AvdName'..." -ForegroundColor Cyan
    Start-Process -FilePath $emulatorPath -ArgumentList "-avd", $AvdName, "-no-boot-anim", "-no-audio" -WindowStyle Minimized
    Write-Host "Esperando conexión del emulador..." -ForegroundColor Gray
    & $adbPath wait-for-device

    $booted = $false
    for ($i = 0; $i -lt 40; $i++) {
        $status = (& $adbPath shell getprop sys.boot_completed 2>$null)
        if ($status -and $status.Trim() -eq "1") {
            $booted = $true
            Write-Host "Emulador completamente iniciado y listo." -ForegroundColor Green
            break
        }
        Start-Sleep -Seconds 2
    }
    if (-not $booted) {
        Write-Host "Aviso: sys.boot_completed no reportó 1 a tiempo; continuando..." -ForegroundColor Yellow
    }
} else {
    Write-Host "Emulador Android activo y conectado." -ForegroundColor Green
}

# 3. Comprobar / Iniciar Servidor Appium
Write-Host "`n[3/6] Verificando servidor Appium en puerto $AppiumPort..." -ForegroundColor Yellow
$appiumOnline = $false
try {
    $res = Invoke-RestMethod -Uri "http://127.0.0.1:$AppiumPort/status" -TimeoutSec 2 -ErrorAction Stop
    if ($res.value.ready -eq $true) { $appiumOnline = $true }
} catch {
    $appiumOnline = $false
}

$appiumProcess = $null
if (-not $appiumOnline) {
    Write-Host "Iniciando servidor Appium en segundo plano..." -ForegroundColor Cyan
    $appiumProcess = Start-Process -FilePath "cmd.exe" -ArgumentList "/c appium --port $AppiumPort" -PassThru -WindowStyle Minimized
    
    for ($i = 0; $i -lt 20; $i++) {
        try {
            Start-Sleep -Seconds 1
            $res = Invoke-RestMethod -Uri "http://127.0.0.1:$AppiumPort/status" -TimeoutSec 2 -ErrorAction Stop
            if ($res.value.ready -eq $true) {
                Write-Host "Servidor Appium iniciado y respondiendo correctamente." -ForegroundColor Green
                break
            }
        } catch { }
    }
} else {
    Write-Host "Servidor Appium ya en ejecución." -ForegroundColor Green
}

# 4. Compilar binario de la app y proyecto E2E
Write-Host "`n[4/6] Compilando aplicación Android (.NET MAUI) y suite de pruebas..." -ForegroundColor Yellow
$appProject = Join-Path $rootDir "TestGithub\TestGithub.csproj"
$e2eProject = Join-Path $rootDir "TestGithub.E2E\TestGithub.E2E.csproj"

dotnet build $appProject -f net9.0-android -c Release --nologo | Out-Null
dotnet build $e2eProject -c Release --nologo | Out-Null

# 5. Ejecutar Pruebas E2E
Write-Host "`n[5/6] Ejecutando pruebas E2E con NUnit..." -ForegroundColor Yellow
dotnet test $e2eProject `
    --configuration Release `
    --logger "console;verbosity=normal"

$testExitCode = $LASTEXITCODE

# 6. Generar Reporte Allure y DocFX
Write-Host "`n[6/6] Generando reportes visuales con Allure y DocFX..." -ForegroundColor Yellow
$allureResults = Join-Path $rootDir "TestGithub.E2E\bin\Release\net9.0\allure-results"
if (-not (Test-Path $allureResults)) {
    $allureResults = Join-Path $rootDir "TestGithub.E2E\bin\Debug\net9.0\allure-results"
}

$allureReportDir = Join-Path $rootDir "docs\allure-report"
if (Test-Path $allureResults) {
    cmd.exe /c "allure generate `"$allureResults`" -o `"$allureReportDir`" --clean"
    Write-Host "Reporte Allure generado en: $allureReportDir" -ForegroundColor Green
} else {
    Write-Host "No se encontraron resultados en $allureResults para generar Allure." -ForegroundColor Yellow
}

Write-Host "Compilando sitio web DocFX..." -ForegroundColor Cyan
dotnet tool restore | Out-Null
dotnet docfx docs/docfx.json

$docfxIndex = Join-Path $rootDir "docs\_site\index.html"
if (Test-Path $docfxIndex) {
    Write-Host "`n[OK] Todo completado con éxito. Portal técnico listo en: $docfxIndex" -ForegroundColor Green
    if (-not $NoBrowser) {
        Start-Process $docfxIndex
    }
}

if ($testExitCode -ne 0) {
    exit $testExitCode
}

