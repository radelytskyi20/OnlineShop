using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OnlineShop.Library.IdentityServer;
using OnlineShop.Library.Options;

namespace OnlineShop.Library.Clients.IdentityServer
{
    public class IdentityServerClient
    {
        public IdentityServerClient(HttpClient client, IOptions<ServiceAdressOptions> options)
        {
            HttpClient = client;
            HttpClient.BaseAddress = new Uri(options.Value.IdentityServer); //указываем http клиенту базовый адресс на identity server
        }

        public HttpClient HttpClient { get; set; }

        public async Task<Token> GetApiToken(IdentityServerApiOptions options)
        {
            var keyValues = new List<KeyValuePair<string, string>>() //задаем значения для http запроса
            {
                new KeyValuePair<string, string>("scope", options.Scope),
                new KeyValuePair<string, string>("client_secret", options.ClientSecret),
                new KeyValuePair<string, string>("grant_type", options.GrantType),
                new KeyValuePair<string, string>("client_id", options.ClientId)
            };

            var content = new FormUrlEncodedContent(keyValues); //создаем http post запрос
            var response = await HttpClient.PostAsync("/connect/token", content); //указываем адресс, по которому мы можем получить токен, и в тело запроса передаем ранее созданный http запрос
            var responseContent = await response.Content.ReadAsStringAsync(); //считываем результат

            var token = JsonConvert.DeserializeObject<Token>(responseContent); //получаем ответ в виде jsona, делаем десериализацию jsona и на основе него создаем объект типа token
            return token; //возвращаем токен
        }
    }
}
