using GranTurismoDataConsole;
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
using Microsoft.Extensions.Options;

namespace GranTurismoDataConsole
{
    internal class Program
    {
        public static IWebDriver Driver { get; private set; }
        public static ChromeOptions chromeOptions = new ChromeOptions();
        static void Main(string[] args)
        {
            using (var context = new GTDataDbContext())
            {
                context.Database.Migrate();
            }


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

            foreach (var href in hrefs)
            {
                saveCarLink(href);

            }

            processCarsLinks();

            Driver.Close();
        }
        static void saveCarLink(string carLink)
        {
            using (var context = new GTDataDbContext())
            {
                var newLink = new Link { Href = carLink, Created = false };
                context.Links.Add(newLink);
                context.SaveChanges();
            }
        }


        static void processCarsLinks()
        {
            var result = false;
            using (var context = new GTDataDbContext())
            {

                var links = context.Links.Where(link => link.Created == false).ToList();
                
                foreach (var link in links)
                {
                    
                    result = processCarLink(link.Href!);

                    if(result)
                    {
                        link.Created = true;
                        context.SaveChanges();
                    }
                    
                }
            }
        }

        static bool processCarLink(string carLink)
        {
            if (string.IsNullOrEmpty(carLink))
            {
                return false;
            }

            var result = false;
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
                // Llamar a la API y si devuelve bien, lo marcamos como true

                if (true) // Check que todas las propiedades necesarias se estén mandando rellenadas
                {
                    
                }

                result = true;
            }
            catch
            {

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
