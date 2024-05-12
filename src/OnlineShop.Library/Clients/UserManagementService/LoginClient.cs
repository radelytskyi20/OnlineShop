using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OnlineShop.Library.Constants;
using OnlineShop.Library.Options;
using OnlineShop.Library.UserManagementService.Models;
using System.Text;

namespace OnlineShop.Library.Clients.UserManagementService
{
    public class LoginClient : UserManagementBaseClient, ILoginClient
    {
        public LoginClient(HttpClient client, IOptions<ServiceAdressOptions> options) 
            : base(client, options) { }

        public async Task<UserManagementServiceToken> GetApiTokenByClientSeceret(IdentityServerApiOptions options) =>
            await SendPostRequest(options, $"login/{LoginControllerRoutes.ByClientSecret}");

        public async Task<UserManagementServiceToken> GetApiTokenByUsernameAndPassword(IdentityServerUserNamePassword options) =>
            await SendPostRequest(options, $"login/{LoginControllerRoutes.ByUsernameAndPassword}");

        private async new Task<UserManagementServiceToken> SendPostRequest<T>(T options, string path)
        {
            var jsonContent = JsonConvert.SerializeObject(options);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var requestResult = await HttpClient.PostAsync(path, httpContent);

            if (requestResult.IsSuccessStatusCode)
            {
                var responseJson = await requestResult.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<UserManagementServiceToken>(responseJson);
                return response;
            }

            return null;
        }
    }
}
