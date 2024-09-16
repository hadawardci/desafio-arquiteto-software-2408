using CashFlow.Consolidated.Infra.Data.Configurations;
using CashFlow.Consolidated.Infra.Data.Contracts;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CashFlow.Consolidated.Infra.Data.Gateways
{
    public class AuthService : IAuthService
    {
        private string _token = string.Empty;
        private DateTime _tokenExpiration;
        private readonly TimeSpan _refreshBeforeExpiration = TimeSpan.FromMinutes(-1);
        private readonly HttpClient _httpClient;
        private readonly ApplicationUserValue _authUser;

        public AuthService(
            IHttpClientFactory httpClientFactory,
            ApplicationUserValue authUser)
        {
            _httpClient = httpClientFactory.CreateClient("AuthService");
            _authUser = authUser;
        }

        public async Task<string> GetTokenAsync(CancellationToken cancellationToken)
        {

            if (string.IsNullOrWhiteSpace(_token) || DateTime.UtcNow.Add(_refreshBeforeExpiration) >= _tokenExpiration)
            {
                _token = await RefreshTokenAsync(cancellationToken);
            }
            return _token;
        }

        private async Task<string> RefreshTokenAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsync(_authUser.Url, new FormUrlEncodedContent(_authUser.Data), cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(content)!;
            _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
            return tokenResponse.AccessToken;
        }
    }

    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public required string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

    }

    public class AuthHandler(IAuthService _authService, ApplicationUserValue _authUser) : DelegatingHandler
    {

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _authService.GetTokenAsync(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue(_authUser.TokenType, token);
            return await base.SendAsync(request, cancellationToken);
        }
    }

}
