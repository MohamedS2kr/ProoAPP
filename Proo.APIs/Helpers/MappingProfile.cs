using AutoMapper;
using Proo.APIs.Dtos.Identity;
using Proo.Core.Entities;

namespace Proo.APIs.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DriverDto , Driver>().ReverseMap();
        }
    }
}
