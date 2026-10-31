using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace TestGithub.E2E
{
    /// <summary>
    /// Pruebas E2E de interfaz gráfica para la pantalla principal de la aplicación MAUI.
    /// </summary>
    [TestFixture]
    [AllureNUnit]
    [AllureSuite("Pruebas E2E Android")]
    [AllureSubSuite("Pantalla Principal")]
    public class MainPageE2ETests : BaseE2ETest
    {
        /// <summary>
        /// Comprueba que al abrir la aplicación los elementos de bienvenida y el botón se renderizan con su texto por defecto.
        /// </summary>
        [Test]
        [AllureSeverity(SeverityLevel.critical)]
        [Description("Verifica que los elementos principales de MainPage se cargan correctamente al iniciar la aplicación.")]
        public void Verificar_Carga_Inicial_De_La_App()
        {
            AllureApi.Step("Esperar a que cargue la interfaz principal", () =>
            {
                IWebElement button = FindButton();
                Assert.That(button, Is.Not.Null);
                Assert.That(button.Text, Does.Contain("Click me"));
            });

            AllureApi.Step("Verificar visibilidad del encabezado", () =>
            {
                IWebElement headline = FindHeadline();
                Assert.That(headline, Is.Not.Null);
                Assert.That(headline.Text, Does.Contain("Hello, World!"));
            });

            CaptureScreenshot("Pantalla_Inicial_Cargada");
        }

        /// <summary>
        /// Comprueba la interactividad del botón y la actualización dinámica del contador en la interfaz de usuario.
        /// </summary>
        [Test]
        [AllureSeverity(SeverityLevel.normal)]
        [Description("Verifica que pulsar el botón incrementa el contador a 'Clicked 1 time' y 'Clicked 2 times'.")]
        public void Verificar_Incremento_Contador_Al_Pulsar_Boton()
        {
            IWebElement button = FindButton();

            AllureApi.Step("Verificar estado inicial del botón", () =>
            {
                Assert.That(button.Text, Is.EqualTo("Click me"));
            });

            AllureApi.Step("Pulsar el botón por primera vez", () =>
            {
                button.Click();
                Assert.That(button.Text, Is.EqualTo("Clicked 1 time"));
            });

            AllureApi.Step("Pulsar el botón por segunda vez", () =>
            {
                button.Click();
                Assert.That(button.Text, Is.EqualTo("Clicked 2 times"));
            });

            CaptureScreenshot("Boton_Pulsado_Dos_Veces");
        }

        private IWebElement FindButton()
        {
            if (Driver == null || Wait == null)
            {
                throw new InvalidOperationException("El controlador de Appium no está inicializado.");
            }

            try
            {
                return Wait.Until(d => d.FindElement(MobileBy.AccessibilityId("CounterBtn")));
            }
            catch (WebDriverTimeoutException)
            {
                return Wait.Until(d => d.FindElement(By.XPath("//android.widget.Button")));
            }
        }

        private IWebElement FindHeadline()
        {
            if (Driver == null || Wait == null)
            {
                throw new InvalidOperationException("El controlador de Appium no está inicializado.");
            }

            try
            {
                return Wait.Until(d => d.FindElement(MobileBy.AccessibilityId("HeadlineLabel")));
            }
            catch (WebDriverTimeoutException)
            {
                return Wait.Until(d => d.FindElement(By.XPath("//android.widget.TextView[contains(@text, 'Hello, World!')]")));
            }
        }
    }
}
