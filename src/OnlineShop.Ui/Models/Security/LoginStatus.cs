namespace OnlineShop.Ui.Models.Security
{
    public class LoginStatus
    {
        public Token Token { get; set; } = new();
        public User User { get; set; } = new();
    }
}
