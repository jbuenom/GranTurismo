using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace GranTurismoData
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--disable-search-engine-choice-screen");

            IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Navigate().GoToUrl("https://gran-turismo.fandom.com/wiki/Gran_Turismo_Wiki");
            driver.FindElement(By.ClassName("NN0_TB_DIsNmMHgJWgT7U")).Click();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
            //IWebElement linkGt7 = driver.FindElement(By.CssSelector("a[href*='GT7_Portal']"));
            //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
            //wait.Until(d => linkGt7.Displayed);
            driver.FindElement(By.CssSelector("a[href*='GT7_Portal']")).Click();



            // Go to Car List
            //var carList = By.CssSelector("#mw-content-text > div > div > ul > li:nth-child(1) > a");
            //wait.Until(d => d.FindElement(carList)).Click();
            // Go to Track list
            //driver.FindElement(By.CssSelector("#mw-content-text > div > div > ul > li:nth-child(4) > a")).Click();
        }
    }
}
