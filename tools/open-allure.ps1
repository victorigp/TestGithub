<#
.SYNOPSIS
    Abre el reporte interactivo de Allure en el navegador web predeterminado.
.DESCRIPTION
    1. Si existe el reporte compilado en docs/allure-report, lanza el servidor local con 'allure open'.
    2. Si no existe el reporte compilado pero sí allure-results, lo genera y sirve al vuelo con 'allure serve'.
    3. Si no existe ninguno, informa al usuario para que ejecute primero las pruebas E2E.
#>

$rootDir = Split-Path -Parent $PSScriptRoot
Set-Location $rootDir

$reportDir = Join-Path $rootDir "docs\allure-report"
$resultsRelease = Join-Path $rootDir "TestGithub.E2E\bin\Release\net9.0\allure-results"
$resultsDebug = Join-Path $rootDir "TestGithub.E2E\bin\Debug\net9.0\allure-results"

if (Test-Path (Join-Path $reportDir "index.html")) {
    Write-Host "Iniciando servidor web de Allure en segundo plano y abriendo navegador..." -ForegroundColor Cyan
    cmd.exe /c "allure open `"$reportDir`""
} elseif (Test-Path $resultsRelease) {
    Write-Host "Generando y abriendo reporte Allure temporal desde $resultsRelease..." -ForegroundColor Cyan
    cmd.exe /c "allure serve `"$resultsRelease`""
} elseif (Test-Path $resultsDebug) {
    Write-Host "Generando y abriendo reporte Allure temporal desde $resultsDebug..." -ForegroundColor Cyan
    cmd.exe /c "allure serve `"$resultsDebug`""
} else {
    Write-Host "`n[AVISO] No se encontró ningún reporte previo de Allure." -ForegroundColor Yellow
    Write-Host "Ejecuta primero la batería de pruebas E2E mediante el botón 'Test E2E' o '.\tools\run-e2e.ps1'." -ForegroundColor Yellow
}

