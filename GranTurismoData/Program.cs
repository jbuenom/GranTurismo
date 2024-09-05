using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;

namespace GranTurismoData
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--disable-search-engine-choice-screen");

            IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://gran-turismo.fandom.com/wiki/Gran_Turismo_Wiki");
            // Close cookies modal
            driver.FindElement(By.ClassName("NN0_TB_DIsNmMHgJWgT7U")).Click();

            // Go to GT7 Portal
            var cssLinkGt7 = By.CssSelector("a[href*='GT7_Portal']");
            IWebElement linkGt7 = driver.FindElement(cssLinkGt7);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
            wait.Until(d => linkGt7.Displayed);
            driver.FindElement(cssLinkGt7).Click();

            // Go to Car List
            var cssCarList = By.CssSelector("#mw-content-text > div > div > ul > li:nth-child(1) > a");
            IWebElement carList = driver.FindElement(cssCarList);
            wait.Until(d => carList.Displayed);
            driver.FindElement(cssCarList).Click();

            var cars = driver.FindElements(By.ClassName("wikitable"))[0].Text;
            

            // Go to Track list
            //driver.FindElement(By.CssSelector("#mw-content-text > div > div > ul > li:nth-child(4) > a")).Click();
            driver.Close();
        }
    }
}
