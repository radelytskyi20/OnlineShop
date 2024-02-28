namespace OnlineShop.Library.Options
{
    /// <summary>
    /// Represents the model to store options to get access token for Api scope of IdentityServer.
    /// We get values for this model from appsettings.json by using IOptions implementation
    /// </summary>
    public class IdentityServerApiOptions
    {
        public const string SectionName = nameof(IdentityServerApiOptions);
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public string GrantType { get; set; }
    }
}
