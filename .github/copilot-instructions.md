# Propósito del proyecto

Este proyecto es una prueba aislada del pipeline de calidad de SIGAPEA_RF. No es la aplicación real.

## Objetivo

Validar en un repositorio personal de GitHub que funcionan:
- El workflow `.github/workflows/quality.yml` (compilación, `dotnet format`).
- Un self-hosted runner ejecutándose en la máquina local.
- El envío de correo al propietario cuando el job de calidad falla (paso automatico de github).

## Contexto

- Proyecto .NET MAUI mínimo, .NET 9, SDK `9.0.315` (`global.json`).
- Solución `TestGithub.sln`. Proyecto `TestGithub/TestGithub.csproj`.
- Sin lógica de negocio, sin EF Core, sin DevExpress, sin SQL Server.
- `Directory.Build.props` idéntico al de SIGAPEA_RF: SonarAnalyzer.CSharp 10.35.0.4138, `TreatWarningsAsErrors=true`, `AnalysisLevel=latest-recommended`.
- El workflow `quality.yml` usa `TestGithub.sln` en lugar de `SIGAPEA_RF.sln`.

## Reglas para el agente

- No añadir funcionalidad de negocio. Solo cambios necesarios para validar el pipeline.
- No introducir secretos ni credenciales en archivos, commits ni ejemplos.
- No modificar `Directory.Build.props` ni `.editorconfig` para ocultar warnings. Corregir el código.
- Mantener `quality.yml` alineado con el de SIGAPEA_RF, salvo el nombre de la solución y el asunto del correo.
- Responder en español, de forma concisa.
- Comentarios en español impersonal.
