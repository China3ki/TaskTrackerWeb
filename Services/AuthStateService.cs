using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using TaskTrackerWeb.Models;

namespace TaskTrackerWeb.Services
{
    public class AuthStateService(ProtectedSessionStorage sessionStorage) : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage = sessionStorage;
        private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var sessionExist = await _sessionStorage.GetAsync<UserSession>("userSession");
                var sessionResult = sessionExist.Success ? sessionExist.Value : null;

                if (sessionResult == null)
                {
                    return new AuthenticationState(_anonymous);
                }

                // Time
                if (sessionResult.ExpiresTime < DateTime.UtcNow)
                {
                    await DestroyAuthenticationState();
                    return new AuthenticationState(_anonymous);
                }

                sessionResult.ExpiresTime = DateTime.UtcNow.AddMinutes(30);

                ClaimsPrincipal claims = CreateSessionStorage(sessionResult);
                return new AuthenticationState(claims);
            }
            catch
            {
                return new AuthenticationState(_anonymous);
            }
        }
        public async Task CreateAuthenticationState(UserSession userSession)
        {
            try
            {
                userSession.ExpiresTime = DateTime.UtcNow.AddMinutes(30);
                await _sessionStorage.SetAsync("userSession", userSession);
                ClaimsPrincipal claims = CreateSessionStorage(userSession);
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claims)));
            } catch
            {
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
            }
        }
        public async Task DestroyAuthenticationState()
        {
            await _sessionStorage.DeleteAsync("userSession");
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }
        private ClaimsPrincipal CreateSessionStorage(UserSession userSession)
        {
            ClaimsPrincipal claims = new(new ClaimsIdentity(new[]
            {
                new Claim("id", userSession.Id.ToString(), ClaimValueTypes.Integer32),
                new Claim(ClaimTypes.Name, userSession.Name),
                new Claim(ClaimTypes.Surname, userSession.Surname),
                new Claim("image", userSession.Image ?? string.Empty),
                new Claim("admin", userSession.Admin.ToString(), ClaimValueTypes.Boolean)
            }, "Auth"));
            return claims;
        }
    }
}
