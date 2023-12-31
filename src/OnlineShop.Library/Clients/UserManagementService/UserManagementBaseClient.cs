using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OnlineShop.Library.Options;
using OnlineShop.Library.UserManagementService.Responses;
using System.Text;

namespace OnlineShop.Library.Clients.UserManagementService
{
    /// <summary>
    /// Represents a client for interacting with UserManagementService(Roles and Users controllers)
    /// </summary>
    public class UserManagementBaseClient : IDisposable
    {
        public UserManagementBaseClient(HttpClient client, IOptions<ServiceAdressOptions> options)
        {
            HttpClient = client;
            HttpClient.BaseAddress = new Uri(options.Value.UserManagementService);
        }

        public HttpClient HttpClient { get; set; }

        /// <summary>
        /// Sends a POST request to the UserManagementService and handles the response
        /// </summary>
        protected async Task<IdentityResult> SendPostRequest<TRequest>(TRequest request, string path)
        {
            var jsonContent = JsonConvert.SerializeObject(request); //serialize request to json
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var requestResult = await HttpClient.PostAsync(path, httpContent); //send http post request to user management service(base address + request address)

            IdentityResult result;

            if (requestResult.IsSuccessStatusCode) //if result is success => deserialize response from user management service to IdentityResultDto and handle it
            {
                var responseJson = await requestResult.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<IdentityResultDto>(responseJson);
                result = HandleResponse(response);
            }
            else //if result is not success => create IdentityResult with error code and description from user management service
            {
                result = IdentityResult.Failed(
                    new IdentityError()
                    {
                        Code = requestResult.StatusCode.ToString(),
                        Description = requestResult.ReasonPhrase
                    });
            }

            return result;
        }
        protected async Task<UserManagementServiceResponse<TResult>> SendGetRequest<TResult>(string request)
        {
            var requestResult = await HttpClient.GetAsync(request);

            UserManagementServiceResponse<TResult> result;

            if (requestResult.IsSuccessStatusCode)
            {
                var response = await requestResult.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(response)) //if response is empty => create UserManagementServiceResponse with error code and description from user management service
                {
                    result = new UserManagementServiceResponse<TResult>()
                    {
                        Code = requestResult.StatusCode.ToString(),
                        Description = requestResult.ReasonPhrase
                    };
                }
                else //if response is success => deserialize response from user management service to UserManagementServiceResponse and return it
                {
                    var payload = JsonConvert.DeserializeObject<TResult>(response);
                    result = new UserManagementServiceResponse<TResult>()
                    {
                        Code = requestResult.StatusCode.ToString(),
                        Description = requestResult.ReasonPhrase,
                        Payload = payload
                    };
                }
            }
            else //if result is not success => create UserManagementServiceResponse with error code and description from user management service
            {
                result = new UserManagementServiceResponse<TResult>()
                {
                    Code = requestResult.StatusCode.ToString(),
                    Description = requestResult.ReasonPhrase
                };
            }

            return result;
        }

        public void Dispose()
        {
            HttpClient.Dispose();
        }

        /// <summary>
        /// Handles the response from the UserManagementService
        /// </summary>
        /// <param name="response">The response from the UserManagementService</param>
        /// <returns>The IdentityResult based on the response</returns>
        private IdentityResult HandleResponse(IdentityResultDto response)
        {
            if (response.Succeeded)
                return IdentityResult.Success;
            else
                return IdentityResult.Failed(response.Errors.ToArray());
        }
    }
}
