namespace OnlineShop.Library.Options
{
    //конфигурационная опция которая содержи адресса всех необходимых серверов
    public class ServiceAdressOptions
    {
        public const string SectionName = nameof(ServiceAdressOptions);
        public string IdentityServer { get; set; }
        public string UserManagementService { get; set; }
        public string OrdersService { get; set; }
        public string ArticlesService { get; set; }
    }
}
