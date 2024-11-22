using AutoMapper;
using Proo.Core.Contract.Dtos.Passenger;
using Proo.Core.Entities;

namespace Proo.APIs.Helpers
{
    public class ProductPictureURLResolver : IValueResolver<ApplicationUser, ProfileDto, string>
    {
        private readonly IConfiguration _configuration;

        public ProductPictureURLResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Resolve(ApplicationUser source, ProfileDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.ProfilePictureUrl))
            {
                return $"{_configuration["ApiBaseUrl"]}/files/ProfilePicture/{source.ProfilePictureUrl}";
            }
            return string.Empty;
        }
    }
}
