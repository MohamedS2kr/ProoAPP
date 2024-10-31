using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Proo.APIs.Dtos.Rides;
using Proo.APIs.Dtos;
using Proo.APIs.Errors;
using Proo.Core.Entities;
using static Proo.APIs.Dtos.ApiToReturnDtoResponse;
using System.Security.Claims;
using AutoMapper;
using Proo.APIs.Hubs;
using Proo.Core.Contract.Passenger_Contract;
using Proo.Core.Contract.RideService_Contract;
using Proo.Core.Contract;
using Microsoft.EntityFrameworkCore;
using Proo.Service._RideService;
using Proo.Core.Specifications.BidSpecifications;
using Proo.Service.Nearby_Driver_Service;
using Proo.Core.Contract.Driver_Contract;

namespace Proo.APIs.Controllers
{
    
    public class FastRideController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDriverRepository _driverRepo;
        private readonly IHubContext<RideHub> _hubContext;
        private readonly IRideAcceptanceService _rideAcceptanceService;
        private readonly IHubContext<FastRideHub> _fastRideHub;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPassengerRepository _passengerRepo;
        private readonly INearbyDriverService _nearbyDriversService;
        private const string Passenger = "passenger";
        private const string driver = "driver";
        public FastRideController(IUnitOfWork unitOfWork
                , IDriverRepository driverRepo,
                IHubContext<RideHub> hubContext, IRideAcceptanceService rideAcceptanceService, IHubContext<FastRideHub> fastRidehub,
                UserManager<ApplicationUser> userManager
                , IPassengerRepository passengerRepo
                , INearbyDriverService nearbyDriversService
                
            )
        {
            _unitOfWork = unitOfWork;
            _driverRepo = driverRepo;
            _hubContext = hubContext;
            _rideAcceptanceService = rideAcceptanceService;
            _fastRideHub = fastRidehub;
            _userManager = userManager;
            _passengerRepo = passengerRepo;
            _nearbyDriversService = nearbyDriversService;
        }
        
        [Authorize(Roles = Passenger)]
        [HttpPost("FastRideRequest")]
     
        public async Task<ActionResult<ApiToReturnDtoResponse>> RideRequest(RideRequestDto request)
        {
            // 1- check passenger is exist 
            var userPhoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == userPhoneNumber);

            var passenger = await _passengerRepo.GetByUserId(user.Id);
            if (passenger is null) return BadRequest(new ApiResponse(400, "The Passenger is not Exist."));


            // 2- check passenger has ongoing trip request 
            var rideRequest = await _unitOfWork.RideRequestRepository.GetActiveTripRequestForPassenger(passenger.Id);
            if (rideRequest is not null) return BadRequest(new ApiResponse(400, "Customer has already a requested trip."));

            // 3- check passenger has ongoing trips

            var unfinishedTrip = await _unitOfWork.RideRepository.GetActiveTripForPassenger(passenger.Id);
            if (unfinishedTrip is not null) return BadRequest(new ApiResponse(400, "Customer has already an ongoing trip."));

            //4- store ride in ride table in database 
            var rideRequestModel = new RideRequests
            {
                PickupAddress = request.PickupAddress,
                PickupLatitude = request.PickupLatitude,
                PickupLongitude = request.PickupLongitude,
                DropoffAddress = request.DropOffAddress,
                DropoffLatitude = request.DropoffLatitude,
                DropoffLongitude = request.DropoffLongitude,
                Category = request.Category,
                CreatedAt = DateTime.Now,
                PassengerId = passenger.Id,
                Status = RideRequestStatus.Requested,
                EstimatedPrice = request.FarePrice, // TODO --> double - decimal 
                //paymentMethod  TODO
            };

            _unitOfWork.Repositoy<RideRequests>().Add(rideRequestModel);
            var count = await _unitOfWork.CompleteAsync();
            if (count <= 0) return BadRequest(new ApiResponse(400));

            // 5- find the nearby drivers Ids  and check driver gender 
            if (request.DriverGenderSelection == GenderType.FemaleOnly && user.Gender == Gender.Male)
                return BadRequest(new ApiResponse(400, "This Feature not supported for males"));

            List<RideNotificationDto> rideNotificationDtos = new List<RideNotificationDto>();
            var notifiedDrivers = new List<Guid>();
            
            if (request.Category == "FastTripe")
            {
                var nearbyDriverIds = await _nearbyDriversService.GetNearbyAvailableDriversAsync(request.PickupLatitude, request.PickupLongitude, 2, 20, request.DriverGenderSelection.ToString());

                if (nearbyDriverIds == null || !nearbyDriverIds.Any())
                {
                    int driverIndex = 0;
                    var isAccepted = false;
                    
                    do
                    {
                        // التحقق إذا كانت هناك قائمة للسائقين المتاحين ولم يتم قبول الطلب بعد
                        if (driverIndex >= nearbyDriverIds.Count)
                        {
                            break; // إنهاء الحلقة إذا انتهت قائمة السائقين
                        }

                        var driverId = nearbyDriverIds[driverIndex];
                        
                        if (notifiedDrivers.Contains(driverId))
                        {
                            driverIndex++;
                            continue;
                        }
                        var notifications = new RideNotificationDto
                        {
                            PickupLat = rideRequestModel.PickupLatitude,
                            PickupLng = rideRequestModel.PickupLongitude,
                            PickupAddress = rideRequestModel.PickupAddress,
                            DropOffLat = rideRequestModel.DropoffLatitude,
                            DropOffLng = rideRequestModel.DropoffLongitude,
                            DropOffAddress = rideRequestModel.DropoffAddress,
                            FarePrice = rideRequestModel.EstimatedPrice,
                            PassengerId = rideRequestModel.PassengerId,
                            Picture = user.ProfilePictureUrl ?? "",
                            Name = user.FullName ?? "",
                            NumberOfTrips = await _unitOfWork.RideRepository.GetRidesCountForPassenger(rideRequestModel.PassengerId),
                        };

                        rideNotificationDtos.Add(notifications);

                        // إرسال الإشعار للسائق الحالي
                        await _hubContext.Clients.User(driverId.ToString()).SendAsync("ReceiveFastRideRequest", notifications);
                        
                        notifiedDrivers.Add(driverId);
                        // انتظار قبول السائق مع مهلة زمنية
                        var taskSource = _rideAcceptanceService.GetOrAddRideAcceptanceSource(driverId.ToString());
                        isAccepted = await Task.WhenAny(taskSource.Task, Task.Delay(TimeSpan.FromSeconds(15))) == taskSource.Task && taskSource.Task.Result;


                        if (isAccepted)
                        {
                            rideRequestModel.Status = RideRequestStatus.CUSTOMER_ACCEPTED; //Driver لحد ما نعدل ونخليه 
                            var count01 = await _unitOfWork.CompleteAsync();
                            if (count01 <= 0) return BadRequest(new ApiResponse(400));
                            break; // خروج من الحلقة لأن السائق قبل الطلب
                        }
                        
                        driverIndex++;
                        

                    } while (!isAccepted && driverIndex < nearbyDriverIds.Count );
                }
                else
                {
                    return BadRequest(new ApiResponse(400, "No drivers available nearby."));
                }
            }
            else
            {
                var nearbyDriverIds = await _nearbyDriversService.GetNearbyAvailableDriversAsync(request.PickupLatitude, request.PickupLongitude, 5, 20, request.DriverGenderSelection.ToString());
                if (nearbyDriverIds == null || !nearbyDriverIds.Any())
                    return BadRequest(new ApiResponse(400, "No drivers available nearby."));
                // 6- Notify Drivers using signalR
                foreach (var id in nearbyDriverIds)
                {
                    var notifications = new RideNotificationDto
                    {
                        PickupLat = rideRequestModel.PickupLatitude,
                        PickupLng = rideRequestModel.PickupLongitude,
                        PickupAddress = rideRequestModel.PickupAddress,
                        DropOffLat = rideRequestModel.DropoffLatitude,
                        DropOffLng = rideRequestModel.DropoffLongitude,
                        DropOffAddress = rideRequestModel.DropoffAddress,
                        FarePrice = rideRequestModel.EstimatedPrice,
                        PassengerId = rideRequestModel.PassengerId,
                    };
                    rideNotificationDtos.Add(notifications);

                    // send the notification to nearby driver 
                    await _hubContext.Clients.User(id.ToString()).SendAsync("ReceiveRideRequest", notifications);

                }
            }
           
            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "The Request Data succ and Notifi the drivers",
                    StatusCode = StatusCodes.Status200OK,
                    Body = rideNotificationDtos
                }
            };
            return Ok(response);
        }
        
        [Authorize(Roles = driver)]
        [HttpPost("Accept-FastRide-Request")]
        public async Task<ActionResult> AcceptRideRequest([FromBody] AcceptRideRequestDto acceptRideRequestDto)
        {
            var phoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            if (user is null) return BadRequest(new ApiResponse(401));  // this validation is not important 

            var driver = await _driverRepo.getByUserId(user.Id);
            //var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var driver = await _unitOfWork.Repositoy<Driver>().GetDriverOrPassengerByIdAsync(driverId);

            if (driver is null) return BadRequest(new ApiResponse(400, "Driver does not exist."));

            // Step 1: Check if the driver has an ongoing ride request
            var ongoingRideRequest = await _unitOfWork.RideRequestRepository.GetActiveTripRequestForDriver(driver.Id);
            if (ongoingRideRequest is not null) return BadRequest(new ApiResponse(400, "Driver has an ongoing ride request."));

            // Step 2: Check if the driver has ongoing trips
            var ongoingTrip = await _unitOfWork.RideRepository.GetActiveTripForDriver(driver.Id);
            if (ongoingTrip is not null) return BadRequest(new ApiResponse(400, "Driver has an ongoing trip."));

            // Step 3: Get and validate the ride request
            var rideRequest = await _unitOfWork.Repositoy<RideRequests>().GetByIdAsync(acceptRideRequestDto.RideRequestId);
            if (rideRequest is null || rideRequest.Status != RideRequestStatus.Requested)
                return BadRequest(new ApiResponse(400, "Ride request is not available."));

            // Check if the ride request is still open (not expired)
            var oneMinuteAgo = DateTime.Now.AddMinutes(-1);
            if (rideRequest.LastModifiedAt < oneMinuteAgo) return BadRequest(new ApiResponse(400, "Ride request is expired."));

            // Step 4: Update ride request status
            rideRequest.Status = RideRequestStatus.CUSTOMER_ACCEPTED;
            rideRequest.DriverId = driver.Id;
            rideRequest.LastModifiedAt = DateTime.Now;

            _unitOfWork.Repositoy<RideRequests>().Update(rideRequest);

            // Step 5: Update driver status to Unavailable
            driver.Status = DriverStatus.InRide;
            _unitOfWork.Repositoy<Driver>().Update(driver);

            // Save changes
            var count = await _unitOfWork.CompleteAsync();
            if (count <= 0) return BadRequest(new ApiResponse(400, "Error occurred while saving changes to the database."));

            // Notify passenger about acceptance (you can add SignalR notification here)
            //await _hubContext.Clients.User(rideRequest.PassengerId).SendAsync("RideRequestAccepted", new { rideRequest.Id, }); 

           
            return Ok(new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Ride request accepted successfully.",
                    StatusCode = StatusCodes.Status200OK,
                   
                }
            });
        }
    }
}
