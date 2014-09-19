using NUnit.Framework;
using System;
using System.Web;
using System.Text;
using System.Net;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace BrowserTesting
{
    [TestFixture()]
    public class BrowserstackTest
    {
        private IWebDriver driver;

        [SetUp]
        public void Init()
        {
            DesiredCapabilities capability = DesiredCapabilities.Firefox();
            capability.SetCapability("browserstack.user", "stelios8");
            capability.SetCapability("browserstack.key", "RawpC5BEniaAtaeeqyzz");

            driver = new RemoteWebDriver(
              new Uri("http://hub.browserstack.com/wd/hub/"), capability
            );
        }

        [Test]
        public void TestCase()
        {
            driver.Navigate().GoToUrl("http://www.google.com");
            StringAssert.Contains("Google", driver.Title);
            IWebElement query = driver.FindElement(By.Name("q"));
            query.SendKeys("Browserstack");
            query.Submit();
        }

        [TearDown]
        public void Cleanup()
        {
            driver.Quit();
        }
    }
}