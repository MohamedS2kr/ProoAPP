using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Proo.Core.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Service.CachService
{
    public class CachService : ICachService
    {
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;

        public CachService( IDistributedCache cache, IConfiguration configuration)
        {
            _cache = cache;
            _configuration = configuration;
        }
            public async Task  CacheOtpAsync(string phone, string otp)
        {
            var cacheKey = phone;
            await _cache.SetStringAsync(cacheKey, otp, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });

        }

        public async Task<string> GetCachedOtpAsync(string phone)
        {
            var cacheKey = phone;
            var Cachedotp = await _cache.GetStringAsync(cacheKey);
            //  _logger.LogInformation($"Retrieved OTP: {Cachedotp} for PhoneNumber: {phoneNumber} with UserId: {UserId}");
            return Cachedotp;
        }
    }
}
