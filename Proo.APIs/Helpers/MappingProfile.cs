using AutoMapper;
using Proo.APIs.Dtos.Identity;
using Proo.APIs.Dtos.Passenger;
using Proo.APIs.Dtos.Rides;
using Proo.Core.Entities;
using static System.Net.WebRequestMethods;

namespace Proo.APIs.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DriverDto , Driver>().ReverseMap();

            //CreateMap<RideRequestDto, Ride>()
            //    .ForMember(r => r.PickupLocation.Latitude, O => O.MapFrom(s => s.PickupLat))
            //    .ForMember(r => r.PickupLocation.Longitude, O => O.MapFrom(s => s.PickupLng))
            //    .ForMember(r => r.PickupLocation.Address, O => O.MapFrom(s => s.PickupAddress))
            //    .ForMember(r => r.DestinationLocation.Latitude, O => O.MapFrom(s => s.DropOffLat))
            //    .ForMember(r => r.DestinationLocation.Longitude, O => O.MapFrom(s => s.DropOffLng))
            //    .ForMember(r => r.DestinationLocation.Address, O => O.MapFrom(s => s.DropOffAddress)).ReverseMap();
            //
            //CreateMap<RdieToReturnDto, Ride>()
            //    .ForMember(r => r.PickupLocation.Latitude, O => O.MapFrom(s => s.PickupLat))
            //    .ForMember(r => r.PickupLocation.Longitude, O => O.MapFrom(s => s.PickupLng))
            //    .ForMember(r => r.PickupLocation.Address, O => O.MapFrom(s => s.PickupAddress))
            //    .ForMember(r => r.DestinationLocation.Latitude, O => O.MapFrom(s => s.DropOffLat))
            //    .ForMember(r => r.DestinationLocation.Longitude, O => O.MapFrom(s => s.DropOffLng))
            //    .ForMember(r => r.DestinationLocation.Address, O => O.MapFrom(s => s.DropOffAddress))
            //    .ForMember(r => r.Status , O => O.MapFrom(s => s.Status))
            //    .ReverseMap();
            CreateMap<ApplicationUser, ProfileDto>()
                //.ForMember(P => P.ProfilePictureUrl, Option => Option.MapFrom(s =>$"{"https://localhost:7147"}/{s.ProfilePictureUrl}" ));
                .ForMember(P => P.ProfilePictureUrl, Option => Option.MapFrom<ProductPictureURLResolver>());
        }
    }
}
