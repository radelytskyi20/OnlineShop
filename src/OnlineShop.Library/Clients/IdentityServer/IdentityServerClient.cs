using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OnlineShop.Library.IdentityServer;
using OnlineShop.Library.Options;

namespace OnlineShop.Library.Clients.IdentityServer
{
    /// <summary>
    /// Represents a client for interacting with Identity Server
    /// </summary>
    public class IdentityServerClient : IIdentityServerClient
    {
        public IdentityServerClient(HttpClient client, IOptions<ServiceAdressOptions> options)
        {
            HttpClient = client;
            HttpClient.BaseAddress = new Uri(options.Value.IdentityServer); //set base address for http client.
                                                                            //It will be used for all requests => base address + request address
                                                                            //We get base address from options (means config file => appsettings.json)
        }

        public HttpClient HttpClient { get; set; }

        public async Task<Token> GetApiToken(IdentityServerApiOptions options)
        {
            var keyValues = new List<KeyValuePair<string, string>>() //body of http post request => contains necessary data for getting token from identity server
            {
                new KeyValuePair<string, string>("scope", options.Scope),
                new KeyValuePair<string, string>("client_secret", options.ClientSecret),
                new KeyValuePair<string, string>("grant_type", options.GrantType),
                new KeyValuePair<string, string>("client_id", options.ClientId)
            };

            var content = new FormUrlEncodedContent(keyValues); //form url encoded content => content to send in http post request
            var response = await HttpClient.PostAsync("/connect/token", content); //send http post request to identity server => base address + request address (/connect/token is our request address)
            var responseContent = await response.Content.ReadAsStringAsync(); //get response content from identity server

            var token = JsonConvert.DeserializeObject<Token>(responseContent); //deserialize token from response content and return it
            return token;
        }
    }
}
