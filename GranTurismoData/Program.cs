using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GranTurismoData
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--disable-search-engine-choice-screen");

            IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Manage().Window.Maximize();

            // Go to Car List
            driver.Navigate().GoToUrl("https://gran-turismo.fandom.com/wiki/Gran_Turismo_7/Car_List");
            // Accept cookies
            driver.FindElement(By.ClassName("NN0_TB_DIsNmMHgJWgT7U")).Click();
            IReadOnlyList<IWebElement> cars = driver.FindElements(By.CssSelector("td > a[href*='/wiki/']"));
            Actions actions = new Actions(driver);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
            var finalListCars = new List<string>();

            for (int i = 0; i < cars.Count(); i++)
            {
                IReadOnlyList<IWebElement> carsUpdated = driver.FindElements(By.CssSelector("td > a[href*='/wiki/']"));
                wait.Until(d => carsUpdated[i].Displayed);
                actions.MoveToElement(carsUpdated[i]);
                actions.Perform();
                carsUpdated[i].Click();

                // Get Car properties
                var model = driver.FindElements(By.CssSelector("aside > h2"));
                var brandWithProductionInfo = driver.FindElements(By.CssSelector("aside > section:nth-child(3) > div > div"));
                var brandWithoutProductionInfo = new List<IWebElement>();

                if (brandWithProductionInfo.Count() == 0)
                {
                    brandWithoutProductionInfo = driver.FindElements(By.CssSelector("div[data-source='manufacturer'] > div > a")).ToList();
                }

                var power = driver.FindElements(By.CssSelector("div[data-source='power'] > div"));
                var weight = driver.FindElements(By.CssSelector("div[data-source='weight'] > div"));
                var category = driver.FindElements(By.CssSelector("div[data-source='class'] > div"));
                var finalBrand = brandWithProductionInfo.Count() > 0 ? brandWithProductionInfo[0].Text.Split('\r')[0] : brandWithoutProductionInfo[0].Text;
                var finalCategory = category.Count() > 0 ? category[0].Text : "";

                var car = model[0].Text + ' ' + finalBrand + ' ' + power[0].Text.Split(' ')[0] + ' ' + weight[0].Text.Split(' ')[0] + ' ' + finalCategory;

                finalListCars.Add(car);


                if ((i + 1) < cars.Count())
                {
                    driver.Navigate().GoToUrl("https://gran-turismo.fandom.com/wiki/Gran_Turismo_7/Car_List");
                }
            }


            var total = finalListCars.Count();
            driver.Close();

            // Go to Track list
            //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
            //wait.Until(d => cars[0].Displayed);
            //driver.FindElement(By.CssSelector("#mw-content-text > div > div > ul > li:nth-child(4) > a")).Click();
        }
    }
}
