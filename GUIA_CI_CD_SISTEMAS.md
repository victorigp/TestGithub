# Requisitos e Instalación del Self-Hosted Runner (Windows)

Este documento detalla los requisitos técnicos, dependencias de software y pasos de instalación necesarios para que el equipo de **Sistemas / DevOps** configure el runner de GitHub Actions para el proyecto.

---

## 1. Requisitos de la Máquina / Servidor del Runner

Para evitar cuellos de botella de descarga en la nube y asegurar reproducibilidad, el pipeline se ejecuta en un **Runner Self-Hosted (Windows)**.

### Especificaciones recomendadas:
* **Sistema Operativo**: Windows Server 2022 o Windows 10/11 (64-bit).
* **Disco**: Mínimo 30 GB libres.
  * **IMPORTANTE (Longitud de rutas)**: Instalar el runner en la raíz de una unidad (por ejemplo `C:\actions-runner` o `E:\actions-runner`). No anidarlo en subcarpetas profundas para evitar el límite de Windows `MAX_PATH` (260 caracteres), especialmente crítico en compilaciones de Android MAUI.
* **Red / Conectividad**:
  * Salida HTTPS (puerto `443`) hacia:
    * `github.com`
    * `*.actions.githubusercontent.com`
    * `*.blob.core.windows.net` (para descarga de paquetes de acciones)
    * `api.nuget.org` (restauración de paquetes NuGet)

---

## 2. Dependencias de Software Necesarias en el Servidor

Antes de arrancar el runner, la máquina debe tener instaladas las siguientes herramientas:

### 2.1. .NET 9 SDK
* **Versión requerida**: Indicada en `global.json` (actualmente `9.0.315` con directiva `rollForward: latestPatch`).
* **Ubicación**: Instalación estándar de 64 bits en `C:\Program Files\dotnet`.
* Debe estar accesible globalmente en la variable de entorno del sistema `PATH`.
* *Comprobación*:
  ```powershell
  dotnet --list-sdks
  # Debe listar 9.0.315 (o superior dentro de la rama 9.0)
  ```

### 2.2. Workload .NET MAUI para Android
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

### 2.3. PowerShell
* El pipeline utiliza **Windows PowerShell 5.1** (incluido por defecto en Windows) o PowerShell 7 (`pwsh`).
* El script de comprobación de calidad está parametrizado con `shell: powershell` para máxima compatibilidad con cualquier instalación estándar de Windows.

---

## 3. Instalación del GitHub Actions Runner como Servicio de Windows

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
   .\config.cmd --url git remote set-url origin https://github.com/viglesias_indra/SIGAPEA_RF.git --token <TOKEN_PROPORCIONADO>
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
