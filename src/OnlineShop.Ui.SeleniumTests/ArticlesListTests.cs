using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace OnlineShop.Ui.SeleniumTests
{
    public class ArticlesListTests
    {
        [Test]
        public void GIVEN_web_app_WHEN_I_add_article_to_cart_THEN_alert_message_is_shown()
        {
            string username = "yaroslav";
            string password = "Pass_123";

            IWebDriver driver = new FirefoxDriver();
            driver.Navigate().GoToUrl("https://localhost:5011");

            Thread.Sleep(1000);

            var usernameTextBox = driver.FindElement(By.Id("login-form-username"));
            usernameTextBox.SendKeys(username);

            var passwordTextBox = driver.FindElement(By.Id("login-form-password"));
            passwordTextBox.SendKeys(password);

            var submitButton = driver.FindElement(By.Id("login-form-submit-button"));
            submitButton.Click();

            Thread.Sleep(1000);

            var row = driver.FindElement(By.Id("529ef33c-4d92-4e17-f612-08dc896fbb55"));
            var quantityInput = row.FindElement(By.Name("quantity"));

            Thread.Sleep(1000);
            
            quantityInput.Click();
            quantityInput.SendKeys("2");

            var buyButton = row.FindElement(By.Name("btn-buy"));
            buyButton.Click();

            Thread.Sleep(1000);

            IAlert alert = SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent().Invoke(driver);
            Assert.That(alert, Is.Not.Null);
            alert.Accept();

            Thread.Sleep(1000);

            var logoutButton = driver.FindElement(By.Id("login-form-logout-button"));
            logoutButton.Click();

            driver.Quit();
        }
    }
}
