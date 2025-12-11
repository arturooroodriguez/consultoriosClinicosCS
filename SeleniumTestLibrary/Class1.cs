using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTestLibrary
{
    public class Class1
    {
        static void Main(string[] args)
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");

            IWebDriver driver = new ChromeDriver(options);

            try
            {
                driver.Navigate().GoToUrl("http://localhost:50239");
                Console.WriteLine("Sistema abierto correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                Console.WriteLine("Presiona cualquier tecla para cerrar...");
                Console.ReadKey();
                driver.Quit();
            }
        }
    }
}
