using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proo.APIs.Dtos;
using Proo.APIs.Dtos.VehicleModel;
using Proo.APIs.Errors;
using Proo.Core.Contract;
using Proo.Core.Entities;
using Proo.Service.VehicleModelService;
using static Proo.APIs.Dtos.ApiToReturnDtoResponse;

namespace Proo.APIs.Controllers
{
    
    public class VehicleModelController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVehicleModelService _vehicleModelService;
        public VehicleModelController(IUnitOfWork unitOfWork, IVehicleModelService vehicleModelService)
        {
            _unitOfWork = unitOfWork;
            _vehicleModelService = vehicleModelService;
        }

        // POST: api/vehiclemodel
        [HttpPost]
        public async Task<ActionResult<ApiToReturnDtoResponse>> CreateVehicleModel([FromBody] VehicleModelDTO vehicleModelDto)
        {
            if (vehicleModelDto == null)
                return BadRequest(new ApiResponse(400, "Vehicle Model data is null."));

            var vehicleModel = new VehicleModel
            {
                ModelName = vehicleModelDto.ModelName,
                VehicleTypeId = vehicleModelDto.VehicleTypeId
            };

            _unitOfWork.Repositoy<VehicleModel>().Add(vehicleModel);
            var result = await _unitOfWork.CompleteAsync();

            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Error creating vehicle model."));

            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Vehicle model created successfully.",
                    StatusCode = StatusCodes.Status201Created,
                    Body = vehicleModelDto
                }
            };

            return Ok(response);
        }
        [HttpGet("byVehicleType/{vehicleTypeId}")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> GetVehicleModelsByVehicleType(int vehicleTypeId)
        {
            var vehicleModels = await _vehicleModelService.GetVehicleModelsByVehicleTypeIdAsync(vehicleTypeId);
            return Ok(new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Vehicle models retrieved successfully.",
                    StatusCode = StatusCodes.Status200OK,
                    Body = vehicleModels
                }
            });
        }
        // GET: api/vehiclemodel/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> GetVehicleModelById(int id)
        {
            var vehicleModel = await _unitOfWork.Repositoy<VehicleModel>().GetByIdAsync(id);
            if (vehicleModel == null)
                return NotFound(new ApiResponse(404, "Vehicle model not found."));

            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Vehicle model retrieved successfully.",
                    StatusCode = StatusCodes.Status200OK,
                    Body = vehicleModel
                }
            };

            return Ok(response);
        }

        // PUT: api/vehiclemodel/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> UpdateVehicleModel(int id, [FromBody] VehicleModelDTO vehicleModelDto)
        {
            if (vehicleModelDto == null)
                return BadRequest(new ApiResponse(400, "Invalid vehicle model data."));

            var existingVehicleModel = await _unitOfWork.Repositoy<VehicleModel>().GetByIdAsync(id);
            if (existingVehicleModel == null)
                return NotFound(new ApiResponse(404, "Vehicle model not found."));

            existingVehicleModel.ModelName = vehicleModelDto.ModelName;
            existingVehicleModel.VehicleTypeId = vehicleModelDto.VehicleTypeId;
            _unitOfWork.Repositoy<VehicleModel>().Update(existingVehicleModel);
            var result = await _unitOfWork.CompleteAsync();

            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Error updating vehicle model."));

            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Vehicle model updated successfully.",
                    StatusCode = StatusCodes.Status200OK,
                    Body = existingVehicleModel
                }
            };

            return Ok(response);
        }

        // DELETE: api/vehiclemodel/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> DeleteVehicleModel(int id)
        {
            var vehicleModel = await _unitOfWork.Repositoy<VehicleModel>().GetByIdAsync(id);
            if (vehicleModel == null)
                return NotFound(new ApiResponse(404, "Vehicle model not found."));

            _unitOfWork.Repositoy<VehicleModel>().Delete(vehicleModel);
            var result = await _unitOfWork.CompleteAsync();

            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Error deleting vehicle model."));

            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Vehicle model deleted successfully.",
                    StatusCode = StatusCodes.Status200OK,
                    Body = null
                }
            };

            return Ok(response);
        }
    }
}
