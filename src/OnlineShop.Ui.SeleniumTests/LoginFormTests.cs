using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace OnlineShop.Ui.SeleniumTests
{
    [TestFixture]
    public class LoginFormTests
    {
        [Test]
        public void GIVEN_web_app_WHEN_I_provide_correct_login_and_password_THEN_I_login_correctly()
        {
            string expectedHelloMessage = "Hello, Yaroslav!";
            string username = "yaroslav";
            string password = "Pass_123";

            IWebDriver driver = new EdgeDriver();
            driver.Navigate().GoToUrl("https://localhost:5011");

            Thread.Sleep(1000);

            var usernameTextBox = driver.FindElement(By.Id("login-form-username"));
            usernameTextBox.SendKeys(username);

            var passwordTextBox = driver.FindElement(By.Id("login-form-password"));
            passwordTextBox.SendKeys(password);

            var submitButton = driver.FindElement(By.Id("login-form-submit-button"));
            submitButton.Click();

            Thread.Sleep(1000);

            var welcomeMessage = driver.FindElement(By.Id("login-form-welcome-message"));
            Assert.That(expectedHelloMessage, Is.EqualTo(welcomeMessage.Text));

            var logoutButton = driver.FindElement(By.Id("login-form-logout-button"));
            logoutButton.Click();

            driver.Quit();
        }

        [Test]
        public void GIVEN_web_app_and_logged_in_user_WHEN_I_click_logout_button_THEN_I_logout_correctly()
        {
            var username = "yaroslav";
            var password = "Pass_123";

            IWebDriver driver = new EdgeDriver();
            driver.Navigate().GoToUrl("https://localhost:5011");

            Thread.Sleep(1000);

            var usernameTextBox = driver.FindElement(By.Id("login-form-username"));
            usernameTextBox.SendKeys(username);

            var passwordTextBox = driver.FindElement(By.Id("login-form-password"));
            passwordTextBox.SendKeys(password);

            var submitButton = driver.FindElement(By.Id("login-form-submit-button"));
            submitButton.Click();

            Thread.Sleep(1000);

            var logoutButton = driver.FindElement(By.Id("login-form-logout-button"));
            logoutButton.Click();

            Thread.Sleep(1000);

            usernameTextBox = driver.FindElement(By.Id("login-form-username"));
            Assert.That(usernameTextBox.Displayed, Is.True);

            driver.Quit();
        }
    }
}
