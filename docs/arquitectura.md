# Arquitectura y Control de Calidad

Este documento resume la arquitectura técnica del proyecto `TestGithub` y la estrategia de calidad implementada.

---

## 1. Estructura de la Solución

El proyecto está construido sobre **.NET 9 MAUI** enfocado a dispositivos móviles Android:

* **`TestGithub.sln`**: Solución raíz del proyecto.
* **`TestGithub/`**: Proyecto principal .NET MAUI (`net9.0-android`).
  * `MainPage.xaml` / `MainPage.xaml.cs`: Pantalla principal y lógica de interacción.
  * `App.xaml` / `AppShell.xaml`: Contenedor y ciclo de vida de la aplicación.
  * `Resources/`: Fuentes, imágenes e iconos vectoriales.

---

## 2. Estrategia Shift-Left de Calidad

El proyecto adopta un enfoque **Shift-Left**, trasladando la detección de vulnerabilidades y defectos a la fase más temprana del ciclo de vida:

1. **`.editorconfig`**: Estandariza el estilo de código (tabulaciones, nombres, saltos de línea CRLF) y eleva advertencias a nivel de compilación.
2. **`Directory.Build.props`**: 
   * Inyecta de forma centralizada `SonarAnalyzer.CSharp` (versión 10.35+).
   * Activa `TreatWarningsAsErrors=true` para tolerancia cero a defectos.
   * Genera automáticamente el informe de calidad en formato **SARIF 2.1**.
3. **GitHub Code Scanning**:
   * Las alertas de calidad de Roslyn se suben automáticamente a la pestaña **Security → Code scanning** de GitHub.
   * CodeQL complementa el análisis detectando posibles vulnerabilidades de seguridad avanzadas.

