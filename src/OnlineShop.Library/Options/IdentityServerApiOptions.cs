using OnlineShop.Library.Constants;

namespace OnlineShop.Library.Options
{
    //класс для хранения опций для identity server
    public class IdentityServerApiOptions //убрать уже заданные значения 
    {
        public const string SectionName = nameof(IdentityServerApiOptions);
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public string GrantType { get; set; }
    }
}
