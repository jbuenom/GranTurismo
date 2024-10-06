
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GranTurismoDataConsole
{
    internal class Program
    {
        public static IWebDriver Driver { get; private set; }
        public static ChromeOptions chromeOptions = new ChromeOptions();
        public static HttpClient client = new HttpClient();
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
            chromeOptions.AddArguments(["--disable-search-engine-choice-screen", "--headless", "--disable-gpu"]);
            Driver = new ChromeDriver(chromeOptions);
            Driver.Manage().Window.Maximize();
            Driver.Navigate().GoToUrl("https://gran-turismo.fandom.com/wiki/Gran_Turismo_7/Car_List");
            var cookies = Driver.FindElements(By.ClassName("NN0_TB_DIsNmMHgJWgT7U"));

            if (cookies.Count == 1)
            {
                cookies[0].Click();
            }
            IReadOnlyList<IWebElement> cars = Driver.FindElements(By.CssSelector("td > a[href*='/wiki/']"));

            var hrefs = cars
              .Select(link => link.GetAttribute("href"))
              .Where(href => !string.IsNullOrEmpty(href))
              .Distinct();

            foreach (var href in hrefs)
            {
                saveLink(href, DataType.Car);

            }

            ProcessLinks(DataType.Car);
        }
        static void saveLink(string Link, DataType typeLink)
        {
            using (var context = new GTDataDbContext())
            {
                var newLink = new Link { Href = Link, Type = typeLink, Created = false };
                context.Links.Add(newLink);
                context.SaveChanges();
            }
        }


        static async void ProcessLinks(DataType typeLink)
        {
            var result = false;
            using (var context = new GTDataDbContext())
            {

                var links = context.Links.Where(link => link.Created == false && link.Type == typeLink).ToList();
                
                foreach (var link in links)
                {
                    
                    result = typeLink == DataType.Car ? await GetCarData(link.Href!) : await GetCircuitData(link.Href!);

                    if(result)
                    {
                        link.Created = true;
                        context.SaveChanges();
                    }
                    
                }
            }
        }

        static async Task<bool> GetCarData(string carLink)
        {
            if (string.IsNullOrEmpty(carLink))
            {
                return false;
            }

            var result = false;

            try
            {

                Driver.Navigate().GoToUrl(carLink);
                var cookies = Driver.FindElements(By.ClassName("NN0_TB_DIsNmMHgJWgT7U"));

                if (cookies.Count == 1)
                {
                    cookies[0].Click();
                }

                var model = Driver.FindElements(By.CssSelector("aside > h2"))[0].Text;
                var brandWithProductionInfo = Driver.FindElements(By.CssSelector("aside > section:nth-child(3) > div > div"));
                var brandWithoutProductionInfo = new List<IWebElement>();

                if (brandWithProductionInfo.Count() == 0)
                {
                    brandWithoutProductionInfo = Driver.FindElements(By.CssSelector("div[data-source='manufacturer'] > div > a")).ToList();
                }

                var power = Driver.FindElements(By.CssSelector("div[data-source='power'] > div"))[0].Text.Split(' ')[0];
                var weight = Driver.FindElements(By.CssSelector("div[data-source='weight'] > div"))[0].Text.Split(' ')[0];
                var category = Driver.FindElements(By.CssSelector("div[data-source='class'] > div"));
                var finalBrand = brandWithProductionInfo.Count() > 0 ? brandWithProductionInfo[0].Text.Split('\r')[0] : brandWithoutProductionInfo[0].Text;
                var finalCategory = category.Count() > 0 ? category[0].Text : "";


                if (model.Length > 0 && weight.Length > 0 && power.Length > 0) 
                {
                    var car = new Car
                    {
                        Model = model,
                        Brand = finalBrand,
                        Weight = int.Parse(weight.Replace(",","")),
                        Power = int.Parse(power.Replace(",", "")),
                        Category = finalCategory
                    };

                    
                    string url = "https://localhost:7188/Cars";
                    string payload = JsonSerializer.Serialize(car);
                    HttpContent content = new StringContent(payload, Encoding.UTF8 ,"application/json");
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine("There was a problem trying to create a car.");
            }

            return result;
        }

        static void GetCircuits()
        {
            Console.WriteLine("Let's update circuits list...");
            chromeOptions.AddArguments(["--disable-search-engine-choice-screen", "--headless", "--disable-gpu"]);
            Driver = new ChromeDriver(chromeOptions);
            Driver.Manage().Window.Maximize();
            Driver.Navigate().GoToUrl("https://gran-turismo.fandom.com/wiki/Gran_Turismo_7/Track_List");
            var cookies = Driver.FindElements(By.ClassName("NN0_TB_DIsNmMHgJWgT7U"));

            if (cookies.Count == 1)
            {
                cookies[0].Click();
            }
            IReadOnlyList<IWebElement> circuits = Driver.FindElements(By.CssSelector("td > a[href*='/wiki/']"));

            var hrefs = circuits
              .Select(link => link.GetAttribute("href"))
              .Where(href => !string.IsNullOrEmpty(href))
              .Distinct();

            foreach (var href in hrefs)
            {
                saveLink(href, DataType.Circuit);

            }

            ProcessLinks(DataType.Circuit);
        }

        static async Task<bool> GetCircuitData(string circuitLink)
        {
            if (string.IsNullOrEmpty(circuitLink))
            {
                return false;
            }

            var result = false;

            try
            {

                Driver.Navigate().GoToUrl(circuitLink);
                var cookies = Driver.FindElements(By.ClassName("NN0_TB_DIsNmMHgJWgT7U"));

                if (cookies.Count == 1)
                {
                    cookies[0].Click();
                }

                var name = Driver.FindElements(By.CssSelector("aside > h2"));
                var location = Driver.FindElements(By.CssSelector("div[data-source='country'] > div > a"));
                var record = Driver.FindElements(By.CssSelector("div[data-source='fastestlap'] > div"));
                var distance = Driver.FindElements(By.CssSelector("div[data-source='length'] > div"));
                var km = "";
                var recordTime = "";

                if (distance.Count > 0) 
                {
                    var distancePattern = @"(\d+(\.\d+)?)\s*(kilometres|km)";
                    
                    Match match = Regex.Match(distance[0].Text, distancePattern, RegexOptions.IgnoreCase);

                    if (match.Success) 
                    {
                        km = match.Groups[1].Value;
                    }
                    
                }

                if(record.Count > 0)
                {
                    recordTime = record[0].Text;
                }

                if (name.Count > 0 && location.Count > 0 && km.Length > 0)
                {
                    var circuit = new Circuit
                    {
                        Name = name[0].Text,
                        Location = location[0].Text,
                        Distance = float.Parse(km.Replace(".", ",")),
                        Record = recordTime
                    };


                    string url = "https://localhost:7188/Circuits";
                    string payload = JsonSerializer.Serialize(circuit);
                    HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem trying to create a circuit.");
            }

            return result;
        }
    }
}
