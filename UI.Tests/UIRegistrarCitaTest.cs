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
    public class UIRegistrarCitaTest
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



        [Test]
        public void AgregarCitaConPacienteExistente()
        {
            // 1️ Seleccionar especialidad
            var selectEspecialidad = wait.Until(d => d.FindElement(By.Id("inputSelectEspecialidad")));
            var opcionesEspecialidad = selectEspecialidad.FindElements(By.TagName("option"));
            Assert.That(opcionesEspecialidad.Count > 1, Is.True, "No hay especialidades disponibles.");
            opcionesEspecialidad[1].Click(); // selecciona la primera especialidad válida

            // 2️ Seleccionar fecha aleatoria futura (entre 1 y 7 días desde hoy)
            var inputFecha = driver.FindElement(By.Id("fechaSeleccionada"));
            var random = new Random();
            int diasAdelante = random.Next(1, 8); // entre 1 y 7 días hacia adelante
            var fechaAleatoria = DateTime.Today.AddDays(diasAdelante);
            inputFecha.SendKeys(fechaAleatoria.ToString("dd-MM-yyyy")); // formato compatible con input type="date"


            // 3️ Esperar que los horarios se carguen y que al menos un botón "Elegir" esté presente
            var primerBotonElegir = wait.Until(d =>
            {
                var botones = d.FindElements(By.CssSelector("#horariosDisponibles .abrir-modal"));
                foreach (var btn in botones)
                {
                    if (btn.Displayed && btn.Enabled)
                        return btn;
                }
                return null;
            });
            Assert.That(primerBotonElegir, Is.Not.Null, "No hay horarios disponibles para seleccionar.");

            // 4️ Hacer clic en el primer horario disponible
            primerBotonElegir.Click();

            // 5️ Esperar que el modal de nueva cita sea visible
            var modalCita = wait.Until(d =>
            {
                var modal = d.FindElement(By.Id("ModalNuevaCita"));
                return (modal.Displayed) ? modal : null;
            });
            Assert.That(modalCita.Displayed, Is.True, "El modal de nueva cita no se mostró.");

            // 6️ Buscar paciente por DNI
            var inputDni = modalCita.FindElement(By.Id("inputDniPaciente"));
            inputDni.SendKeys("71024415");
            modalCita.FindElement(By.Id("button-addon2")).Click(); // presionar lupa

            var datosPaciente = wait.Until(d =>
            {
                var dp = modalCita.FindElement(By.Id("datosPaciente"));
                return dp.Displayed ? dp : null;
            });
            Assert.That(datosPaciente.Displayed, Is.True, "No se cargaron los datos del paciente.");

            // 7️ Seleccionar tipo de consulta
            var selectTipoConsulta = modalCita.FindElement(By.Id("inputSelectTipoConsulta"));
            var opcionesConsulta = selectTipoConsulta.FindElements(By.TagName("option"));
            Assert.That(opcionesConsulta.Count > 1, Is.True, "No hay tipos de consulta disponibles.");
            opcionesConsulta[1].Click(); // selecciona la primera opción válida

            // 8️ Validar que campos automáticos se llenen
            var inputFechaHora = modalCita.FindElement(By.Id("inputFechaHora"));
            var inputHoraInicio = modalCita.FindElement(By.Id("inputHoraInicio"));
            var inputMedicoNombre = modalCita.FindElement(By.Id("inputMedicoNombre"));

            Assert.That(!string.IsNullOrEmpty(inputFechaHora.GetAttribute("value")), Is.True, "La fecha y hora no se llenaron automáticamente.");
            Assert.That(!string.IsNullOrEmpty(inputHoraInicio.GetAttribute("value")), Is.True, "La hora de inicio no se llenó automáticamente.");
            Assert.That(!string.IsNullOrEmpty(inputMedicoNombre.GetAttribute("value")), Is.True, "El nombre del médico no se llenó automáticamente.");

            // 9️ Guardar cita
            modalCita.FindElement(By.Id("btnGuardarCita")).Click();

            // 10 Manejar el alert correctamente
            try
            {
                // Esperar hasta que el alert aparezca
                var alert = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());

                // Validar el texto del alert
                string alertText = alert.Text.Trim();
                Assert.That(alertText.Contains("Cita registrada exitosamente"), Is.True,
                    $"El texto del alert no es el esperado. Texto real: {alertText}");

                // Aceptar el alert
                alert.Accept();

                Assert.Pass("✅ La cita se registró correctamente y se mostró el mensaje de confirmación.");
            }
            catch (WebDriverTimeoutException)
            {
                Assert.Fail("❌ No se mostró el alert de confirmación después de guardar la cita.");
            }

        }




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
