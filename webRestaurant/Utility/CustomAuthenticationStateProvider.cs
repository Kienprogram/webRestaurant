using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Text.Json;

namespace webRestaurant.Utility
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly ILocalStorageService _localStorageService;

        public CustomAuthenticationStateProvider(IJSRuntime jsRuntime, ILocalStorageService localStorageService)
        {
            _jsRuntime = jsRuntime;
            _localStorageService = localStorageService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorageService.GetItemAsStringAsync("auth_token");
            Console.WriteLine($"Token retrieved: {token}");

            var user = string.IsNullOrEmpty(token)
                ? new ClaimsPrincipal(new ClaimsIdentity())
                : new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));

            return new AuthenticationState(user);
        }

        public void NotifyUserAuthentication(string token)
        {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogout()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }
        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            // Ensure that roles are included in the claims
            var claims = keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())).ToList();

            if (keyValuePairs.ContainsKey("level")) // Ensure this matches the role claim key
            {
                var roles = keyValuePairs["leve"] as JsonElement?; // Use the correct claim key
                if (roles.HasValue && roles.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var role in roles.Value.EnumerateArray())
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.GetString()));
                    }
                }
            }

            return claims;
        }
        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
        private async Task<string> GetJwtToken()
        {
            return await _localStorageService.GetItemAsync<string>("auth_token");
        }
    }
}
