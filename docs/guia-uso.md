# Guía de Uso y CI/CD

Guía rápida de desarrollo, ejecución del runner local y automatización de despliegues.

---

## 1. Compilación y Comprobación Local

Para compilar el proyecto en modo estricto y generar el informe SARIF en local:

```powershell
# Compilación en Release con análisis estático
dotnet build TestGithub.sln --configuration Release
```

Si existe cualquier infracción de estilo o regla de SonarAnalyzer, la compilación se detendrá con código de error 1 indicando el archivo y línea exactos.

---

## 2. Documentación con DocFX

La documentación se gestiona con herramientas locales de .NET (`.config/dotnet-tools.json`):

```powershell
# Restaurar herramientas locales (solo la primera vez)
dotnet tool restore

# Compilar el sitio web estático en docs/_site
dotnet docfx docs/docfx.json

# Compilar y previsualizar en el navegador en tiempo real (http://localhost:8080)
dotnet docfx docs/docfx.json --serve
```

---

## 3. GitHub Actions Runner

El repositorio cuenta con un runner self-hosted para compilaciones locales de Windows/Android:

* **Iniciar Runner**: Utiliza el botón **`▷ Runner`** de la barra de estado de Antigravity (o ejecuta `E:\actions-runner\run.cmd`).
* **Pipelines activos**:
  * **Calidad y Seguridad (`quality.yml`)**: Valida compilación, Roslyn SARIF y CodeQL ante cada push.
  * **Pruebas E2E Android (`e2e.yml`)**: Ejecuta pruebas de integración en el emulador Android, generando artefactos con Allure.
  * **Publicación de Docs (`docs.yml`)**: Compila y publica automáticamente este portal en **GitHub Pages** integrando Cobertura y Allure.

---

## 4. Ejecución de Pruebas E2E Locales (Appium + Allure)

Para ejecutar la batería de pruebas end-to-end de forma automatizada en local:

```powershell
# Ejecuta el script automatizado (inicia emulador, Appium, compila la app y lanza Allure)
.\tools\run-e2e.ps1
```

