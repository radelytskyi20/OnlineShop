namespace OnlineShop.Library.Options
{
    /// <summary>
    /// Represents the model to store uri of all services for OnlineShop.
    /// We get values for this model from appsettings.json by using IOptions implementation
    /// </summary>
    public class ServiceAdressOptions
    {
        public const string SectionName = nameof(ServiceAdressOptions);
        public string IdentityServer { get; set; }
        public string UserManagementService { get; set; }
        public string OrdersService { get; set; }
        public string ArticlesService { get; set; }
        public string ApiService { get; set; }
    }
}
