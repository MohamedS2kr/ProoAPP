using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Proo.APIs.Dtos;
using Proo.APIs.Dtos.Rides;
using Proo.APIs.Errors;
using Proo.APIs.Hubs;
using Proo.Core.Contract;
using Proo.Core.Contract.RideService_Contract;
using Proo.Core.Entities;
using Proo.Infrastructer.Data.Context;
using Proo.Service._RideService;
using static Proo.APIs.Dtos.ApiToReturnDtoResponse;

namespace Proo.APIs.Controllers
{
    
    public class RideController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRideService _rideService;
        private readonly IHubContext<RideHub> _hubContext;

        public RideController(IUnitOfWork unitOfWork 
            , IMapper mapper
            , IRideService rideService ,
            IHubContext<RideHub> hubContext) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _rideService = rideService;
            _hubContext = hubContext;
        }


        [HttpPost("request")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> RideRequest(RideRequestDto request)
        {
            // store ride in ride table in database 
            var RideMapped = _mapper.Map<Ride>(request);
            RideMapped.PassengerId = request.PassengerId;
            RideMapped.Status = RideStatus.Requested;
            RideMapped.CreatedAt = DateTime.UtcNow;

            _unitOfWork.Repositoy<Ride>().Add(RideMapped);
            var count = await _unitOfWork.CompleteAsync();
            if (count <= 0) return BadRequest(new ApiResponse(400));

            // find the nearby drivers 
            var nearbyDrivers = await _rideService.GetNearbyDrivers(request.PickupLat, request.PickupLng, 5);

            // Notify Drivers using signalR
            foreach (var driver in nearbyDrivers)
            {
                var notifications = new RideNotificationDto
                {
                    PickupLat = RideMapped.PickupLocation.Latitude,
                    PickupLng = RideMapped.PickupLocation.Longitude,
                    PickupAddress = RideMapped.PickupLocation.Address,
                    DropOffLat = RideMapped.DestinationLocation.Latitude,
                    DropOffLng = RideMapped.DestinationLocation.Longitude,
                    DropOffAddress = RideMapped.DestinationLocation.Address,
                    FarePrice = RideMapped.FarePrice,
                    PassengerId = RideMapped.PassengerId,

                };

                // send the notification to nearby driver 
                await _hubContext.Clients.User(driver.Id).SendAsync("ReceiveRideRequest", notifications);
            }

            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "The Request Data succ",
                    StatusCode = StatusCodes.Status200OK,
                    Body = new List<object>
                    {
                        _mapper.Map<RdieToReturnDto>(RideMapped)
                    }
                }
            };

            return Ok(response);
        }


        //[HttpPost("calc_price_time_destance")]
        //public async Task<ActionResult<ApiToReturnDtoResponse>> CalcPriceAndTimeAndDestance(calculatePriceAnddectaceDto CalcDto )
        //{
        //    var result = new LocationService().CalculateDestanceAndTimeAndPrice(CalcDto.PickUpLat, CalcDto.PickUpLon, CalcDto.DroppOffLat, CalcDto.DroppOffLon);

        //    if(CalcDto.Category == "Ride")
        //    {
        //        return (new ApiToReturnDtoResponse
        //        {
        //            Data = new DataResponse
        //            {
        //                Mas = "The Price and Time Calculated by dectance and Category [Ride]",
        //                StatusCode = StatusCodes.Status200OK,
        //                Body = new List<object>
        //                {

        //                }
        //            }
        //        });
        //    }
        //    else if(CalcDto.Category == "Comfort")
        //    {

        //    }
        //    else if(CalcDto.Category == "Scoter")
        //    {

        //    }
        //}
        

    }
}
