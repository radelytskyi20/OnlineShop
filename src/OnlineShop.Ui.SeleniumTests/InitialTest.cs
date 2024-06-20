using OpenQA.Selenium.Edge;
using OpenQA.Selenium;

namespace OnlineShop.Ui.SeleniumTests
{
    [TestFixture]
    public class InitialTest
    {
        [Test]
        public void EdgeSession()
        {
            string recivedMessage = "Received!";
            string webForm = "Web form";

            IWebDriver driver = new EdgeDriver();

            driver.Navigate().GoToUrl("https://www.selenium.dev/selenium/web/web-form.html");
            var title = driver.Title;
            Assert.That(webForm, Is.EqualTo(title));

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);

            var textBox = driver.FindElement(By.Name("my-text"));
            var submitButton = driver.FindElement(By.TagName("button"));

            textBox.SendKeys("Selenium");
            submitButton.Click();

            var message = driver.FindElement(By.Id("message"));
            var value = message.Text;
            Assert.That(recivedMessage, Is.EqualTo(value));

            driver.Quit();
        }
    }
}
