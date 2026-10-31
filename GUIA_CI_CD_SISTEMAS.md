# Requisitos e Instalación del Self-Hosted Runner (Windows)
---

## 1. Objetivo

Este documento detalla los requisitos técnicos, dependencias de software y pasos de instalación necesarios para que el equipo de **Sistemas / DevOps** configure el runner de GitHub Actions para el proyecto.

## 2. Requisitos de la Máquina / Servidor del Runner

Para evitar cuellos de botella de descarga en la nube y asegurar reproducibilidad, el pipeline se ejecuta en un **Runner Self-Hosted (Windows)**.

### Especificaciones recomendadas:
* **Sistema Operativo**: Windows Server 2022 o Windows 10/11 (64-bit).
* **Disco**: Mínimo 30 GB libres.
  * **IMPORTANTE (Longitud de rutas)**: Instalar el runner en la raíz de una unidad (por ejemplo `C:\actions-runner`). No anidarlo en subcarpetas profundas para evitar el límite de Windows `MAX_PATH` (260 caracteres), especialmente crítico en compilaciones de Android MAUI.
* **Red / Conectividad**:
  * Salida HTTPS (puerto `443`) hacia:
    * `github.com`
    * `*.actions.githubusercontent.com`
    * `*.blob.core.windows.net` (para descarga de paquetes de acciones)
    * `api.nuget.org` (restauración de paquetes NuGet)

---

## 3. Dependencias de Software Necesarias en el Servidor

Antes de arrancar el runner, la máquina debe tener instaladas las siguientes herramientas:

### 3.1. .NET 9 SDK
* **Versión requerida**: Indicada en `global.json` (actualmente `9.0.315` con directiva `rollForward: latestPatch`).
* **Ubicación**: Instalación estándar de 64 bits en `C:\Program Files\dotnet`.
* Debe estar accesible globalmente en la variable de entorno del sistema `PATH`.
* *Comprobación*:
  ```powershell
  dotnet --list-sdks
  # Debe listar 9.0.315 (o superior dentro de la rama 9.0)
  ```

### 3.2. Workload .NET MAUI para Android
* Necesario para compilar el proyecto Android.
* Instalar ejecutando en una consola con permisos de Administrador:
  ```powershell
  dotnet workload install maui-android
  ```
* *Comprobación*:
  ```powershell
  dotnet workload list
  # Debe figurar 'maui-android'
  ```

### 3.3. PowerShell
* El pipeline utiliza **Windows PowerShell 5.1** (incluido por defecto en Windows) o PowerShell 7 (`pwsh`).
* El script de comprobación de calidad está parametrizado con `shell: powershell` para máxima compatibilidad con cualquier instalación estándar de Windows.

### 3.4. Git for Windows (64-bit)
* **Imprescindible**:
  1. Es la herramienta esencial que el runner utiliza para clonar el repositorio (`actions/checkout@v4`).
  2. **Proporciona los binarios Unix estándar `bash.exe` y `tar.exe`**: Las GitHub Actions oficiales (como `actions/upload-pages-artifact` para DocFX) requieren `bash` y `tar` para empaquetar artefactos web. Al tener Git for Windows, el pipeline utiliza estos binarios directamente sin necesidad de instalar WSL (*Windows Subsystem for Linux*) ni componentes de Linux adicionales en el servidor.
* **Instalación**:
  ```powershell
  winget install --id Git.Git -e --source winget
  ```
  *(O descargando el instalador oficial de https://git-scm.com/)*.
* **Comprobación**:
  ```powershell
  git --version
  # Debe devolver git version 2.x o superior
  ```

### 3.5. GitHub CLI (`gh`)
* **Propósito**: Permite al runner interactuar directamente con la API de GitHub Actions (por ejemplo, para descargar artefactos entre diferentes workflows de forma desacoplada y como respaldo mediante `gh run download`).
* **Instalación**:
  ```powershell
  winget install --id GitHub.cli -e --source winget
  ```
* **Comprobación**:
  ```powershell
  gh --version
  ```

### 3.6. Herramientas de Cobertura y Documentación (DocFX, Coverlet y ReportGenerator)
* **Sin instalación administrativa previa**:
  * **DocFX** y **ReportGenerator**: Se gestionan como herramientas locales del repositorio a través del manifiesto `.config/dotnet-tools.json`. El runner las restaura de forma automática y aislada en cada pipeline mediante `dotnet tool restore`.
  * **Coverlet**: Se gestiona como paquete NuGet estándar (`coverlet.collector`) incluido en los proyectos de test (`*.Test.csproj`).
* **Mantenimiento**: El equipo de Sistemas **no necesita instalar ni actualizar estas herramientas a nivel de sistema operativo**. Solo debe garantizar la conectividad con el repositorio de paquetes NuGet (`api.nuget.org` o el proxy/Nexus interno corporativo).

### 3.7. Herramientas para Pruebas E2E Android (Appium, Allure CLI y Emulador Android)

Para que el runner self-hosted pueda ejecutar las pruebas funcionales de interfaz en la aplicación Android MAUI de forma autónoma y desatendida, deben instalarse y configurarse los siguientes componentes:

#### 3.7.1. Java Runtime (JDK / JBR)
* Requerido por el motor de **Allure CLI** y el SDK de Android.
* Ubicación recomendada: `E:\Android Studio\jbr` (o `C:\Program Files\Java\jdk-xx`).
* Variable de entorno del sistema requerida:
  * `JAVA_HOME = E:\Android Studio\jbr` (agregando `%JAVA_HOME%\bin` al `PATH`).

#### 3.7.2. Android SDK y Herramientas de Plataforma
* Requiere el Android SDK con los paquetes `cmdline-tools`, `platform-tools` (`adb.exe`), `build-tools` y `emulator` (`emulator.exe`).
* Variables de entorno requeridas:
  * `ANDROID_HOME = E:\Android_SDK`
  * `ANDROID_SDK_ROOT = E:\Android_SDK`
  * Añadir al `PATH`: `%ANDROID_HOME%\platform-tools` y `%ANDROID_HOME%\emulator`.

#### 3.7.3. Especificaciones del Emulador Android (AVD: `seypos_scanmax_22d`)
El pipeline de pruebas E2E interactúa contra un dispositivo virtual Android representativo del hardware industrial objetivo (**SEYPOS ScanMax 22D**). El emulador en el servidor debe crearse con las siguientes especificaciones idénticas a las de desarrollo:

| Parámetro | Valor Requerido | Observaciones |
| :--- | :--- | :--- |
| **Nombre del AVD (ID)** | `seypos_scanmax_22d` | Identificador utilizado por Appium y los scripts de CI |
| **Nombre Mostrado** | `SEYPOS ScanMax 22D` | Nombre en interfaz gráfica de Android Studio |
| **Versión de Android / API** | `Android 36 (Google APIs)` | Imagen del sistema `system-images;android-36;google_apis;x86_64` |
| **Arquitectura de CPU / ABI** | `x86_64` | Alto rendimiento con virtualización Intel HAXM / Hyper-V (WHPX) |
| **Resolución de Pantalla** | `720 x 1280` píxeles | Skin personalizada 720x1280 vertical |
| **Densidad de Pantalla (DPI)** | `290 dpi` | Densidad exacta del escáner industrial |
| **Memoria RAM** | `4000 MB` (4 GB) | Evita cierres por falta de memoria en ejecución MAUI |
| **VM Heap** | `256 MB` | Tamaño del heap de la máquina virtual |
| **Partición de Datos / Disco** | `6 GB` (`6442450944` bytes) | Almacenamiento suficiente para APKs y datos de prueba |
| **Tarjeta SD Virtual** | `512 MB` | Almacenamiento externo simulado |
| **Núcleos de CPU** | `4 cores` | Asignación de hilos de emulación |
| **Aceleración GPU** | `Hardware - GLES 2.0 / Auto` | Renderizado rápido de interfaz gráfica |

#### 3.7.4. Script para Creación del Emulador (PowerShell - Sistemas)
El equipo de Sistemas puede crear este emulador de forma 100% desatendida mediante PowerShell:

```powershell
# 1. Instalar la imagen del sistema Android 36 Google APIs (x86_64)
& "$env:ANDROID_HOME\cmdline-tools\latest\bin\sdkmanager.bat" "system-images;android-36;google_apis;x86_64"

# 2. Crear el dispositivo AVD
& "$env:ANDROID_HOME\cmdline-tools\latest\bin\avdmanager.bat" create avd `
    --name "seypos_scanmax_22d" `
    --package "system-images;android-36;google_apis;x86_64" `
    --device "pixel" `
    --force

# 3. Aplicar los parámetros de hardware específicos en su archivo config.ini
$avdConfigPath = "$env:USERPROFILE\.android\avd\seypos_scanmax_22d.avd\config.ini"
$configOverrides = @(
    "avd.ini.displayname=SEYPOS ScanMax 22D",
    "hw.lcd.width=720",
    "hw.lcd.height=1280",
    "hw.lcd.density=290",
    "hw.ramSize=4000",
    "vm.heapSize=256",
    "hw.cpu.ncore=4",
    "hw.gpu.enabled=yes",
    "hw.gpu.mode=auto",
    "disk.dataPartition.size=6442450944",
    "sdcard.size=512M",
    "showDeviceFrame=no"
)
$configOverrides | Out-File -FilePath $avdConfigPath -Encoding utf8 -Append
```

#### 3.7.5. Appium Server
* Instalado globalmente mediante Node.js:
  ```powershell
  npm install -g appium
  ```
* Controlador Android UiAutomator2 instalado en Appium:
  ```powershell
  appium driver install uiautomator2
  ```
* Comprobación:
  ```powershell
  appium driver list
  # Debe figurar: uiautomator2 [installed]
  ```

#### 3.7.6. Allure CLI
* Herramienta de línea de comandos para generar el sitio web HTML estático a partir de los artefactos JSON de prueba:
  ```powershell
  scoop install allure
  # o mediante winget:
  winget install Allure.Allure
  ```
* Comprobación:
  ```powershell
  allure --version
  ```

---

## 4. Instalación del GitHub Actions Runner como Servicio de Windows

Para que el runner esté siempre disponible sin requerir sesiones de usuario abiertas:

1. **Obtener el token del repositorio**:
   * En GitHub: **Settings** → **Actions** → **Runners** → **New self-hosted runner**.
   * Seleccionar **Windows / x64**.

2. **Descarga y despliegue** (ejemplo en `C:\actions-runner`):
   ```powershell
   mkdir C:\actions-runner
   cd C:\actions-runner

   # Descargar la versión más reciente del runner de GitHub
   Invoke-WebRequest -Uri https://github.com/actions/runner/releases/download/v2.338.0/actions-runner-win-x64-2.338.0.zip -OutFile actions-runner.zip

   # Descomprimir
   Add-Type -AssemblyName System.IO.Compression.FileSystem
   [System.IO.Compression.ZipFile]::ExtractToDirectory("$PWD\actions-runner.zip", "$PWD")
   ```

3. **Configuración**:
   ```powershell
   .\config.cmd --url https://github.com/viglesias_indra/SIGAPEA_RF.git --token <TOKEN_PROPORCIONADO>
   ```
   * *Runner name*: Nombre del servidor (pulsar Enter para valor por defecto).
   * *Labels*: Aceptar por defecto (`self-hosted,Windows,X64`).
   * *Work folder*: `_work` (pulsar Enter para valor por defecto).

4. **Instalación como Servicio de Windows (Desatendido)**:
   ```powershell
   .\svc.cmd install
   .\svc.cmd start
   ```
   *(El servicio arrancará automáticamente con el sistema bajo el nombre `actions.runner.<org-repo>.<runner-name>`)*.


---

## 5. Permisos Requeridos en GitHub (Configuración del Repositorio y Organización)

Para que los workflows de CI/CD funcionen correctamente sin bloqueos de seguridad corporativos, el administrador de sistemas/DevOps debe verificar los siguientes permisos:

### 5.1. Permisos de Workflow (`Settings > Actions > General > Workflow permissions`)
* Asegurarse de que esté seleccionado **"Read and write permissions"** (o en su defecto, permitir que los workflows definan sus propios alcances mediante la directiva `permissions:`).

### 5.2. Permisos específicos utilizados por los pipelines
* **Calidad y Seguridad (`quality.yml`):**
  * `security-events: write`: Imprescindible para que el runner pueda cargar los informes SARIF de Roslyn y CodeQL a la pestaña **Security → Code scanning alerts**.
  * `actions: read` y `contents: read`: Para descargar el código y consultar el estado del runner.
* **Pruebas E2E Android (`e2e.yml`):**
  * `actions: read` y `contents: read`: Para descargar el repositorio y subir los artefactos del reporte Allure (`Reporte-Allure-TestGithub`).
* **Documentación Técnica con DocFX (`docs.yml`):**
  * `pages: write`: Requerido para empaquetar y desplegar el portal web generado en **GitHub Pages**.
  * `id-token: write`: Requerido por GitHub para autenticar de forma segura el despliegue del entorno de Pages.
  * `actions: read`: Necesario para consultar y descargar los artefactos de cobertura (`quality.yml`) y pruebas E2E Allure (`e2e.yml`).
  * `contents: read`: Para descargar el código y la documentación del repositorio.

### 5.3. Habilitación de GitHub Pages
* En la organización de GitHub (Indra / Navantia), asegurarse de que la directiva de seguridad corporativa permita el uso de **GitHub Pages** para repositorios privados/internos.
* En el repositorio: **Settings → Pages → Build and deployment → Source**: Seleccionar **`GitHub Actions`**.

### 5.4. Variable de Configuración para CodeQL (`ENABLE_CODEQL`)
Para controlar de forma dinámica la ejecución del análisis de vulnerabilidades y seguridad avanzada de GitHub (CodeQL):

* **Ubicación en GitHub**: **Settings → Secrets and variables → Actions → pestaña Variables → New repository variable**.
  * **Nombre**: `ENABLE_CODEQL`
  * **Valor**: `true`

> [!NOTE]
> **Diferencia entre entornos Indra vs Cliente Final (Navantia):**
> * **Entorno Indra**: **NO es necesario crear esta variable** (o se debe configurar en `false`). La organización interna de Indra no dispone de licencias de *GitHub Advanced Security (GHAS)* para repositorios privados, por lo que CodeQL está deshabilitado para evitar errores de licenciamiento.
> * **Entorno Cliente Final (Navantia)**: **SÍ es necesario crearla con valor `true`**, ya que el cliente dispone del licenciamiento corporativo de CodeQL.
> 
> Si la variable no existe o tiene un valor distinto de `true`, el pipeline de calidad (`quality.yml`) omite los pasos de CodeQL de forma automática y transparente sin interrumpir la compilación ni los tests.
