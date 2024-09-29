using GranTurismoDataConsole.GranTurismoDataDbContext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace GranTurismoDataConsole
{
    internal class Program
    {
        public static IWebDriver Driver { get; private set; }
        public static ChromeOptions chromeOptions = new ChromeOptions();
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddDbContext<GTDataDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
                .BuildServiceProvider();


            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=== Menú ===");
                Console.WriteLine("1. Get cars list");
                Console.WriteLine("2. Get circuits list");
                Console.WriteLine("3. Exit");
                Console.Write("Choose an option: ");

                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        GetCars();
                        break;
                    case "2":
                        GetCircuits();
                        break;
                    case "3":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Try again");
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void GetCars()
        {
            Console.WriteLine("Let's update cars list...");
            chromeOptions.AddArgument("--disable-search-engine-choice-screen");
            Driver = new ChromeDriver(chromeOptions);
            // Go to Car List
            Driver.Manage().Window.Maximize();
            Driver.Navigate().GoToUrl("https://gran-turismo.fandom.com/wiki/Gran_Turismo_7/Car_List");
            // Accept cookies
            Driver.FindElement(By.ClassName("NN0_TB_DIsNmMHgJWgT7U")).Click();
            IReadOnlyList<IWebElement> cars = Driver.FindElements(By.CssSelector("td > a[href*='/wiki/']"));

            var hrefs = cars
              .Select(link => link.GetAttribute("href"))
              .Where(href => !string.IsNullOrEmpty(href))
              .Distinct();

            // Save on db all the link with created false
            foreach (var href in hrefs)
            {
                saveCarLink(href);

            }

            processCarsLinks();

            Driver.Close();
        }
        static void saveCarLink(string carLink)
        {
            //using (var context = new MyDbContext())
            //{
            //    var newCarLink = new Link { Url = carLink };
            //    context.Links.Add(carLink);
            //    context.SaveChanges();
            //}


        }


        static void processCarsLinks()
        {
            //using (var context = new MyDbContext())
            //{
            //    // Obtener todos los enlaces no procesados
            //    var enlaces = context.Enlaces.Where(e => e.Procesado == false).ToList();

            //    foreach (var enlace in enlaces)
            //    {
            //        // Aquí podrías usar Selenium nuevamente para visitar el enlace
            var result = processCarLink("https://gran-turismo.fandom.com/wiki/Alfa_Romeo_MiTo_1.4_T_Sport_%2709");

            //        // Marcar como procesado en la base de datos
            //if(result)
            //{
            //    enlace.Procesado = true;

            //}
            //      
            //        context.SaveChanges();
            //    }
            //}
        }

        static bool processCarLink(string carLink)
        {
            var result = true;
            Driver.Navigate().GoToUrl(carLink);

            try
            {

                var model = Driver.FindElements(By.CssSelector("aside > h2"));
                var brandWithProductionInfo = Driver.FindElements(By.CssSelector("aside > section:nth-child(3) > div > div"));
                var brandWithoutProductionInfo = new List<IWebElement>();

                if (brandWithProductionInfo.Count() == 0)
                {
                    brandWithoutProductionInfo = Driver.FindElements(By.CssSelector("div[data-source='manufacturer'] > div > a")).ToList();
                }

                var power = Driver.FindElements(By.CssSelector("div[data-source='power'] > div"));
                var weight = Driver.FindElements(By.CssSelector("div[data-source='weight'] > div"));
                var category = Driver.FindElements(By.CssSelector("div[data-source='class'] > div"));
                var finalBrand = brandWithProductionInfo.Count() > 0 ? brandWithProductionInfo[0].Text.Split('\r')[0] : brandWithoutProductionInfo[0].Text;
                var finalCategory = category.Count() > 0 ? category[0].Text : "";

                var car = model[0].Text + ' ' + finalBrand + ' ' + power[0].Text.Split(' ')[0] + ' ' + weight[0].Text.Split(' ')[0] + ' ' + finalCategory;

                // Call API to insert car

            }
            catch
            {
                result = false;
            }

            return result;
        }

        static void GetCircuits()
        {
            Console.WriteLine("Let's update circuits list...");
            // Go to Track list
            //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
            //wait.Until(d => cars[0].Displayed);
            //driver.FindElement(By.CssSelector("#mw-content-text > div > div > ul > li:nth-child(4) > a")).Click();
        }
    }
}
