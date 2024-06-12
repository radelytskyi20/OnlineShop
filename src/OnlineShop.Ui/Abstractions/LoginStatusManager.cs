using OnlineShop.Ui.Abstractions.Interfaces;
using OnlineShop.Ui.Models.Security;

namespace OnlineShop.Ui.Abstractions
{
    public class LoginStatusManager : ILoginStatusManager
    {
        private readonly HttpClient _client;
        private LoginStatus _loginStatus = new();

        public LoginStatus LoginStatus => _loginStatus;
        public event EventHandler? LoginStatusHasChanged;

        public LoginStatusManager(HttpClient client) { _client = client; }

        public async Task<bool> LogIn(string username, string password)
        {
            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("password", password)
                });

                var response = await _client.PostAsync("login", content);
                if (!response.IsSuccessStatusCode)
                    return false;

                _loginStatus.Token = await response.Content.ReadFromJsonAsync<Token>() ?? new();

                //set token in header of http client => we use one http client for all requests so all requests will be authenticated
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue( 
                    scheme: "Bearer",
                    parameter: _loginStatus.Token.AccessToken
                );

                _loginStatus.User = await _client.GetFromJsonAsync<User>($"users?userName={username}") ?? new();
                LoginStatusHasChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
            catch (Exception) { return false; }
        }

        public async Task<bool> LogOut()
        {
            _loginStatus.Token = new();
            _loginStatus.User = new();

            LoginStatusHasChanged?.Invoke(this, EventArgs.Empty);
            return await Task.FromResult(true);
        }
    }
}
