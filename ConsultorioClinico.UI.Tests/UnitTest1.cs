using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Diagnostics;

namespace UI.Tests
{
    public class SistemaUITests
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private Process webAppProcess;

        [SetUp]
        public void Setup()
        {
            // 1️⃣ Iniciar la aplicación web
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "run --urls http://localhost:50239",
                WorkingDirectory = @"C:\Users\Daniel\Cosultorio", // Cambia por la ruta de tu proyecto web
                UseShellExecute = true
            };
            webAppProcess = Process.Start(startInfo);

            // 2️⃣ Esperar a que el servidor esté disponible
            WaitForServer("http://localhost:50239");

            // 3️⃣ Configurar Chrome
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--start-maximized");

            driver = new ChromeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // 4️⃣ Navegar a la página
            driver.Navigate().GoToUrl("http://localhost:50239/");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();

            // Cerrar la aplicación web
            if (!webAppProcess.HasExited)
            {
                webAppProcess.Kill(); // Elimina el argumento 'true', ya que Kill() no acepta argumentos
            }
        }

        [Test]
        public void AbrirSistema_DeberiaMostrarMenuPrincipal()
        {
            var menuPrincipal = wait.Until(d =>
            {
                var element = d.FindElement(By.Id("lista-pacientes-container"));
                return element.Displayed ? element : null;
            });

            Assert.That(menuPrincipal.Displayed, "El menú principal no se muestra correctamente.");
        }

        private void WaitForServer(string url, int timeoutSeconds = 30)
        {
            var client = new System.Net.Http.HttpClient();
            var end = DateTime.Now.AddSeconds(timeoutSeconds);

            while (DateTime.Now < end)
            {
                try
                {
                    var response = client.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        return; // servidor iniciado
                }
                catch
                {
                    // ignorar errores mientras el servidor se inicia
                }
                System.Threading.Thread.Sleep(1000);
            }

            throw new Exception($"No se pudo conectar al servidor en {url} después de {timeoutSeconds} segundos.");
        }
    }
}
