using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OnlineShop.Library.IdentityServer;
using OnlineShop.Library.Options;
using static System.Formats.Asn1.AsnWriter;

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
            var keyValuesContent = new List<KeyValuePair<string, string>>() //body of http post request => contains necessary data for getting token from identity server
            {
                new KeyValuePair<string, string>("scope", options.Scope),
                new KeyValuePair<string, string>("client_secret", options.ClientSecret),
                new KeyValuePair<string, string>("grant_type", options.GrantType),
                new KeyValuePair<string, string>("client_id", options.ClientId)
            };

            return await GetTokenRequest(keyValuesContent); //send http post request to identity server and get token
        }

        public async Task<Token> GetApiToken(IdentityServerUserNamePassword options)
        {
            var keyValuesContent = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("scope", options.Scope),
                new KeyValuePair<string, string>("grant_type", options.GrantType),
                new KeyValuePair<string, string>("client_id", options.ClientId),
                new KeyValuePair<string, string>("username", options.UserName),
                new KeyValuePair<string, string>("password", options.Password)
            };

            return await GetTokenRequest(keyValuesContent);
        }

        protected async Task<Token> GetTokenRequest(IEnumerable<KeyValuePair<string, string>> keyValuesContent)
        {
            try
            {
                var content = new FormUrlEncodedContent(keyValuesContent); //form url encoded content => content to send in http post request
                var response = await HttpClient.PostAsync("/connect/token", content); //send http post request to identity server => base address + request address (/connect/token is our request address)
                var responseContent = await response.Content.ReadAsStringAsync(); //get response content from identity server

                var token = JsonConvert.DeserializeObject<Token>(responseContent); //deserialize token from response content and return it
                return token;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
