namespace OnlineShop.Ui.Models.Security
{
    public class Token
    {
        public string AccessToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public bool IsExpired => DateTime.UtcNow > CreatedAt.AddSeconds(ExpiresIn);
        public bool IsLoggedIn => !IsExpired && !string.IsNullOrEmpty(AccessToken);

        public Token() { CreatedAt = DateTime.UtcNow; }
    }
}
