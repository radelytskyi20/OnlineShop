using Microsoft.JSInterop;
using Newtonsoft.Json;
using OnlineShop.Ui.Abstractions.Interfaces;
using OnlineShop.Ui.Models.Common;
using System.Text;

namespace OnlineShop.Ui.Abstractions
{
    public class UsersManager : IUsersManager
    {
        private readonly HttpClient _client;
        private readonly IJSRuntime _jSRuntime;
        public UsersManager(HttpClient client, IJSRuntime jSRuntime) 
        { 
            _client = client;
            _jSRuntime = jSRuntime;
        }

        public async Task UpdateAsync(User user)
        {
            var jsonContent = JsonConvert.SerializeObject(user);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var requestResult = await _client.PostAsync("users/update", httpContent);

            if (requestResult.IsSuccessStatusCode)
            {
                UserEntityHasChanged?.Invoke(this, EventArgs.Empty);
                await _jSRuntime.InvokeVoidAsync("alert", "User info has been updated");
                return;
            }

            await _jSRuntime.InvokeVoidAsync("alert", $"User info update failed. Response: {requestResult.StatusCode}");
        }

        public event EventHandler? UserEntityHasChanged;
    }

}


