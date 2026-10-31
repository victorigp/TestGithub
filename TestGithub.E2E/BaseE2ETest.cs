using Allure.Net.Commons;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;

namespace TestGithub.E2E
{
    /// <summary>
    /// Clase base para las pruebas E2E con Appium en Android y reporte Allure.
    /// </summary>
    public abstract class BaseE2ETest
    {
        /// <summary>
        /// Instancia del controlador de Android proporcionado por Appium.
        /// </summary>
        protected AndroidDriver? Driver { get; private set; }

        /// <summary>
        /// Instancia de espera explícita para sincronización de elementos.
        /// </summary>
        protected WebDriverWait? Wait { get; private set; }

        /// <summary>
        /// Inicializa el controlador de Appium antes de cada prueba.
        /// </summary>
        [SetUp]
        public virtual void SetUp()
        {
            string appiumUrl = Environment.GetEnvironmentVariable("APPIUM_SERVER_URL") ?? "http://127.0.0.1:4723/";
            string apkPath = ResolveApkPath();

            AppiumOptions options = new()
            {
                PlatformName = "Android",
                AutomationName = "UiAutomator2",
                DeviceName = Environment.GetEnvironmentVariable("APPIUM_DEVICE_NAME") ?? "emulator-5554",
                App = apkPath
            };

            options.AddAdditionalAppiumOption("appium:appPackage", "com.companyname.testgithub");
            options.AddAdditionalAppiumOption("appium:appActivity", "crc645139d31f056161d8.MainActivity");
            options.AddAdditionalAppiumOption("appium:appWaitActivity", "*");
            options.AddAdditionalAppiumOption("appium:noReset", true);
            options.AddAdditionalAppiumOption("appium:autoGrantPermissions", true);
            options.AddAdditionalAppiumOption("appium:newCommandTimeout", 120);

            Driver = new AndroidDriver(new Uri(appiumUrl), options, TimeSpan.FromMinutes(2));
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
        }

        /// <summary>
        /// Libera los recursos del controlador y captura la pantalla en caso de fallo.
        /// </summary>
        [TearDown]
        public virtual void TearDown()
        {
            try
            {
                if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed && Driver != null)
                {
                    Screenshot screenshot = Driver.GetScreenshot();
                    AllureApi.AddAttachment(
                        $"Fallo_{TestContext.CurrentContext.Test.Name}",
                        "image/png",
                        screenshot.AsByteArray);
                }
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"No se pudo capturar la pantalla en el TearDown: {ex.Message}");
            }
            finally
            {
                Driver?.Quit();
                Driver?.Dispose();
                Driver = null;
            }
        }

        /// <summary>
        /// Captura una captura de pantalla y la adjunta al reporte de Allure.
        /// </summary>
        /// <param name="name">Nombre identificativo del adjunto.</param>
        protected void CaptureScreenshot(string name)
        {
            if (Driver != null)
            {
                Screenshot screenshot = Driver.GetScreenshot();
                AllureApi.AddAttachment(name, "image/png", screenshot.AsByteArray);
            }
        }

        /// <summary>
        /// Localiza la ruta del archivo APK compilado en Release o Debug.
        /// </summary>
        private static string ResolveApkPath()
        {
            string? explicitPath = Environment.GetEnvironmentVariable("E2E_APK_PATH");
            if (!string.IsNullOrEmpty(explicitPath) && File.Exists(explicitPath))
            {
                return Path.GetFullPath(explicitPath);
            }

            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string projectRoot = Path.GetFullPath(Path.Combine(currentDir, "..", "..", "..", ".."));

            string[] candidates =
            [
                Path.Combine(projectRoot, "TestGithub", "bin", "Release", "net9.0-android", "com.companyname.testgithub-Signed.apk"),
                Path.Combine(projectRoot, "TestGithub", "bin", "Debug", "net9.0-android", "com.companyname.testgithub-Signed.apk"),
                Path.Combine(projectRoot, "TestGithub", "bin", "Debug", "net9.0-android", "com.companyname.testgithub.apk")
            ];

            foreach (string candidate in candidates)
            {
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            throw new FileNotFoundException(
                "No se encontró el binario APK compilado para las pruebas E2E. " +
                "Ejecute 'dotnet build TestGithub/TestGithub.csproj -f net9.0-android -c Release' antes de ejecutar los tests.");
        }
    }
}
