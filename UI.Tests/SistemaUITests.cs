using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace UI.Tests
{
    [TestFixture]
    public class SistemaUITests
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private Process iisProcess;

        private string proyectoWebRuta = @"C:\Users\Daniel\Cosultorio\Capa1_Presentacion.Web.AspNet.ModuloPrincipal";
        private int puerto = 50239;

        [SetUp]
        public void Setup()
        {
            string iisExe = @"C:\Program Files\IIS Express\iisexpress.exe";
            string argumentos = $"/path:\"{proyectoWebRuta}\" /port:{puerto}";

            iisProcess = Process.Start(iisExe, argumentos);

            WaitForServer($"http://localhost:{puerto}");

            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--start-maximized");

            driver = new ChromeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            driver.Navigate().GoToUrl($"http://localhost:{puerto}/");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();

            if (iisProcess != null && !iisProcess.HasExited)
            {
                iisProcess.Kill();
                iisProcess.WaitForExit();
            }
        }

        /*[Test]
        public void AgregarPacienteConContactoEmergencia()
        {
            // 1️ Ir a Pacientes
            var linkPacientes = wait.Until(d =>
                d.FindElement(By.CssSelector("a.nav-link[data-controller='GestionarPacientes']"))
            );
            linkPacientes.Click();

            // 2️ Abrir modal Agregar Paciente
            var btnAgregarPaciente = wait.Until(d =>
                d.FindElement(By.CssSelector("a.btn[data-bs-target='#modalAgregarPaciente']"))
            );
            btnAgregarPaciente.Click();

            // 3️ Esperar modal visible
            var modalPaciente = wait.Until(d =>
                d.FindElement(By.Id("modalAgregarPaciente"))
            );

            // 4️ Completar datos del paciente
            driver.FindElement(By.Id("PacienteNombres")).SendKeys("Cris");
            driver.FindElement(By.Id("PacienteApellidos")).SendKeys("Asmat");
            driver.FindElement(By.Id("PacienteDNI")).SendKeys("71027613");
            driver.FindElement(By.Id("PacienteTelefono")).SendKeys("987654321");
            driver.FindElement(By.Id("PacienteCorreoElectronico")).SendKeys("sam.perez@example.com");
            driver.FindElement(By.Id("PacienteDireccion")).SendKeys("Av. Siempre Viva 123");
            driver.FindElement(By.Id("PacienteFechaNacimiento")).SendKeys("01-01-1990");
            driver.FindElement(By.Id("PacienteSeguro")).SendKeys("Seguro XYZ");

            // 5️ Agregar contacto de emergencia
            var btnAgregarContacto = driver.FindElement(By.Id("btnAgregarContacto"));
            btnAgregarContacto.Click();

            // 6 Esperar modal de contacto completamente interactuable
            var modalContacto = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("modalAgregarContacto")));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("contactoNombre")));

            // 7 Llenar datos del contacto
            driver.FindElement(By.Id("contactoNombre")).SendKeys("Debbie Perez");
            driver.FindElement(By.Id("contactoRelacion")).SendKeys("Hermana");
            driver.FindElement(By.Id("contactoTelefono")).SendKeys("948092846");

            // 8 Guardar contacto
            modalContacto.FindElement(By.CssSelector("button.btn-primary[type='submit']")).Click();

            // 9 Esperar a que modal de contacto se cierre
            wait.Until(d => !modalContacto.Displayed);

            // 10 Guardar paciente
            modalPaciente.FindElement(By.CssSelector("button.btn-primary[type='submit']")).Click();

            // 11 Manejar la alerta
            try
            {
                IAlert alert = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());
                Console.WriteLine("Alerta: " + alert.Text);
                alert.Accept();
            }
            catch (WebDriverTimeoutException) { }

            // Esperar a que modal paciente se cierre
            wait.Until(d => !modalPaciente.Displayed);

            Assert.Pass("Paciente y contacto agregado correctamente.");
        }
        */

        

        private void WaitForServer(string url, int timeoutSeconds = 30)
        {
            var client = new HttpClient();
            var end = DateTime.Now.AddSeconds(timeoutSeconds);

            while (DateTime.Now < end)
            {
                try
                {
                    var response = client.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        return;
                }
                catch { Thread.Sleep(1000); }
                Thread.Sleep(1000);
            }

            throw new Exception($"No se pudo conectar al servidor en {url} después de {timeoutSeconds} segundos.");
        }
    }
}
