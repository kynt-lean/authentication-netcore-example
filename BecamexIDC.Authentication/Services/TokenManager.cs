using System;
using System.Linq;
using System.Threading.Tasks;
using BecamexIDC.Authentication.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace BecamexIDC.Authentication.Services
{
    public interface ITokenManager
    {
        Task<bool> IsCurrentActiveToken();
        Task DeactivateCurrentAsync();
        Task<bool> IsActiveAsync(string token);
        Task DeactivateAsync(string token);
    }
    public class TokenManager : ITokenManager
    {
        private readonly IDistributedCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<JwtSettings> _jwtOptions;

        public TokenManager(IDistributedCache cache,
                IHttpContextAccessor httpContextAccessor,
                IOptions<JwtSettings> jwtOptions
            )
        {
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
            _jwtOptions = jwtOptions;
        }

        public async Task<bool> IsCurrentActiveToken()
            => await IsActiveAsync(GetCurrentAsync());

        public async Task DeactivateCurrentAsync()
            => await DeactivateAsync(GetCurrentAsync());

        public async Task<bool> IsActiveAsync(string token)
            => await _cache.GetStringAsync(GetKey(token)) == null;

        public async Task DeactivateAsync(string token)
            => await _cache.SetStringAsync(GetKey(token),
                " ", new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =_jwtOptions.Value.TokenLifetime
                });

        private string GetCurrentAsync()
        {
            var authorizationHeader = _httpContextAccessor
                .HttpContext.Request.Headers["authorization"];

            return authorizationHeader == StringValues.Empty
                ? string.Empty
                : authorizationHeader.Single().Split(" ").Last();
        }

        private static string GetKey(string token)
            => $"tokens:{token}:deactivated";
    }
}