using Microsoft.AspNetCore.Mvc;
using Proo.Core.Contract;
using Proo.Core.Contract.Dtos;
using Proo.Core.Contract.Dtos.VehicleType;
using Proo.Core.Contract.Errors;
using Proo.Core.Entities;
using Proo.Service.VehicleTypeService;
using DataResponse = Proo.Core.Contract.Dtos.ApiToReturnDtoResponse.DataResponse;

namespace Proo.APIs.Controllers
{

    public class VehicleTypeController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVehicleTypeService _vehicleTypeService;

        public VehicleTypeController(IUnitOfWork unitOfWork, IVehicleTypeService vehicleTypeService)
        {
            _unitOfWork = unitOfWork;
            _vehicleTypeService = vehicleTypeService;
        }

        // POST: api/vehicletype
        [HttpPost]
        public async Task<ActionResult<ApiToReturnDtoResponse>> CreateVehicleType([FromBody] VehicleTypeDTO vehicleTypeDto)
        {
            if (vehicleTypeDto == null)
                return BadRequest(new ApiResponse(400, "Vehicle Type data is null."));

            var vehicleType = new VehicleType
            {
                TypeName = vehicleTypeDto.TypeName,
                CategoryOfVehicleId = vehicleTypeDto.CategoryOfVehicleId
            };

            _unitOfWork.Repositoy<VehicleType>().Add(vehicleType);
            var result = await _unitOfWork.CompleteAsync();

            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Error creating vehicle type."));

            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Vehicle type created successfully.",
                    StatusCode = StatusCodes.Status201Created,
                    Body = vehicleTypeDto
                }
            };

            return Ok(response);
        }


        // GET: api/vehicletype/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> GetVehicleTypeById(int id)
        {
            var vehicleType = await _unitOfWork.Repositoy<VehicleType>().GetByIdAsync(id);
            if (vehicleType == null)
                return NotFound(new ApiResponse(404, "Vehicle type not found."));
            var VehicletypeDto = new ReturnVehicleTypeDTO
            {
                Id = vehicleType.Id,
                TypeName = vehicleType.TypeName,
                CategoryOfVehicleId = vehicleType.CategoryOfVehicleId
            };
            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Vehicle type retrieved successfully.",
                    StatusCode = StatusCodes.Status200OK,
                    Body = VehicletypeDto
                }
            };

            return Ok(response);
        }
        [HttpGet("byCategory/{categoryId}")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> GetVehicleTypesByCategory(int categoryId)
        {
            var vehicleTypes = await _vehicleTypeService.GetVehicleTypesByCategoryIdAsync(categoryId);
            if (!vehicleTypes.Any())
                return NotFound(new ApiResponse(404, "No vehicle types found for the specified category."));
            var vehicleTypeDtos = vehicleTypes.Select(vehicletype => new ReturnVehicleTypeDTO
            {
                Id = vehicletype.Id,
                TypeName = vehicletype.TypeName,
                CategoryOfVehicleId = vehicletype.CategoryOfVehicleId
            }).ToList();
            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Vehicle types retrieved successfully.",
                    StatusCode = StatusCodes.Status200OK,
                    Body = vehicleTypeDtos
                }
            };

            return Ok(response);
        }

        // PUT: api/vehicletype/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> UpdateVehicleType(int id, [FromBody] VehicleTypeDTO vehicleTypeDto)
        {
            if (vehicleTypeDto == null)
                return BadRequest(new ApiResponse(400, "Invalid vehicle type data."));

            var existingVehicleType = await _unitOfWork.Repositoy<VehicleType>().GetByIdAsync(id);
            if (existingVehicleType == null)
                return NotFound(new ApiResponse(404, "Vehicle type not found."));

            existingVehicleType.TypeName = vehicleTypeDto.TypeName;
            existingVehicleType.CategoryOfVehicleId = vehicleTypeDto.CategoryOfVehicleId;
            _unitOfWork.Repositoy<VehicleType>().Update(existingVehicleType);
            var result = await _unitOfWork.CompleteAsync();

            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Error updating vehicle type."));
            var vehicletypeDto = new ReturnVehicleTypeDTO
            {
                Id = existingVehicleType.Id,
                TypeName = existingVehicleType.TypeName,
                CategoryOfVehicleId = existingVehicleType.CategoryOfVehicleId
            };
            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Vehicle type updated successfully.",
                    StatusCode = StatusCodes.Status200OK,
                    Body = vehicletypeDto
                }
            };

            return Ok(response);
        }

        // DELETE: api/vehicletype/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> DeleteVehicleType(int id)
        {
            var vehicleType = await _unitOfWork.Repositoy<VehicleType>().GetByIdAsync(id);
            if (vehicleType == null)
                return NotFound(new ApiResponse(404, "Vehicle type not found."));

            _unitOfWork.Repositoy<VehicleType>().Delete(vehicleType);
            var result = await _unitOfWork.CompleteAsync();

            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Error deleting vehicle type."));

            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Vehicle type deleted successfully.",
                    StatusCode = StatusCodes.Status200OK,
                    Body = null
                }
            };

            return Ok(response);
        }
    }
}
