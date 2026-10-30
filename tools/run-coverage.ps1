<#
.SYNOPSIS
    Ejecuta las pruebas unitarias y genera el reporte visual de cobertura de código.
.DESCRIPTION
    1. Ejecuta dotnet test sobre TestGithub.Test recolectando cobertura en formato Cobertura XML.
    2. Utiliza ReportGenerator para transformar el XML en un sitio web HTML interactivo.
    3. Abre automáticamente el reporte HTML en el navegador predeterminado.
#>

[CmdletBinding()]
param(
    [switch]$NoBrowser
)

$ErrorActionPreference = "Stop"

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "  TestGithub - Ejecución de Pruebas y Cobertura de Código " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

$rootDir = Split-Path -Parent $PSScriptRoot
Set-Location $rootDir

# 1. Asegurar herramientas locales (.NET Tools)
Write-Host "`n[1/3] Restaurando herramientas locales (.NET Tools)..." -ForegroundColor Yellow
dotnet tool restore | Out-Null

# 2. Ejecutar pruebas unitarias con recolección de cobertura
Write-Host "`n[2/3] Ejecutando pruebas unitarias y recolectando cobertura con Coverlet..." -ForegroundColor Yellow
$resultsDir = Join-Path $rootDir "TestResults"
if (Test-Path $resultsDir) {
    Remove-Item -Path $resultsDir -Recurse -Force -ErrorAction SilentlyContinue
}

$testProject = Join-Path $rootDir "TestGithub.Test\TestGithub.Test.csproj"
dotnet test $testProject `
    --configuration Release `
    --collect:"XPlat Code Coverage" `
    --results-directory $resultsDir `
    --logger "console;verbosity=normal"

if ($LASTEXITCODE -ne 0) {
    Write-Host "`n[ERROR] Las pruebas unitarias han fallado. Revisa los errores anteriores." -ForegroundColor Red
    exit $LASTEXITCODE
}

# 3. Generar informe HTML con ReportGenerator
Write-Host "`n[3/3] Generando reporte visual HTML con ReportGenerator..." -ForegroundColor Yellow
$reportDir = Join-Path $resultsDir "CoverageReport"
$coverageXmlPattern = "TestResults\**\coverage.cobertura.xml"

dotnet reportgenerator `
    "-reports:$coverageXmlPattern" `
    "-targetdir:$reportDir" `
    "-reporttypes:Html;HtmlSummary;MarkdownSummaryGithub" `
    "-title:TestGithub - Reporte de Cobertura de Pruebas Unitarias"

$indexHtml = Join-Path $reportDir "index.html"
if (Test-Path $indexHtml) {
    Write-Host "`n[OK] Reporte generado con éxito en: $indexHtml" -ForegroundColor Green
    if (-not $NoBrowser) {
        Write-Host "Abriendo reporte en el navegador..." -ForegroundColor Cyan
        Start-Process $indexHtml
    }
} else {
    Write-Host "`n[ADVERTENCIA] No se encontró el archivo de reporte index.html." -ForegroundColor Yellow
}

