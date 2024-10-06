﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proo.APIs.Dtos;
using Proo.APIs.Dtos.CategoryOfVehicle;
using Proo.APIs.Errors;
using Proo.Core.Contract;
using Proo.Core.Entities;
using static Proo.APIs.Dtos.ApiToReturnDtoResponse;

namespace Proo.APIs.Controllers
{
   
    public class CategoryOfVehicleController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryOfVehicleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // POST: api/category
        [HttpPost]
        public async Task<ActionResult<ApiToReturnDtoResponse>> CreateCategory([FromBody] CategoryDTO category)
        {
            if (category == null)
                return BadRequest(new ApiResponse(400, "Category is null."));
            var cat = new CategoryOfVehicle
            {
                Name = category.Name,
                Description = category.Description
            };
            _unitOfWork.Repositoy<CategoryOfVehicle>().Add(cat);
            var result = await _unitOfWork.CompleteAsync();

            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Error creating category."));

            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Category created successfully.",
                    StatusCode = StatusCodes.Status201Created,
                    Body = category
                }
            };
            return Ok(response);
        }

        // GET: api/category
        [HttpGet]
        public async Task<ActionResult<ApiToReturnDtoResponse>> GetAllCategories()
        {
            var categories = await _unitOfWork.Repositoy<CategoryOfVehicle>().GetAll();
            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Retrieved all categories successfully.",
                    StatusCode = StatusCodes.Status200OK,
                    Body = categories
                }
            };

            return Ok(response);
        }

        // GET: api/category/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> GetCategoryById(int id)
        {
            var category = await _unitOfWork.Repositoy<CategoryOfVehicle>().GetByIdAsync(id);
            if (category == null)
                return NotFound(new ApiResponse(404, "Category not found."));

            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Category retrieved successfully.",
                    StatusCode = StatusCodes.Status200OK,
                    Body = category
                }
            };

            return Ok(response);
        }

        // PUT: api/category/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> UpdateCategory(int id, [FromBody] CategoryDTO category)
        {
            if (category == null )
                return BadRequest(new ApiResponse(400, "Invalid category data."));

            var existingCategory = await _unitOfWork.Repositoy<CategoryOfVehicle>().GetByIdAsync(id);

            if (existingCategory == null)
                return NotFound(new ApiResponse(404, "Category not found."));

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            _unitOfWork.Repositoy<CategoryOfVehicle>().Update(existingCategory);
            var result = await _unitOfWork.CompleteAsync();

            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Error updating category."));

            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Category updated successfully.",
                    StatusCode = StatusCodes.Status200OK,
                    Body = existingCategory
                }
            };

            return Ok(response);
        }
        // DELETE: api/category/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> DeleteCategory(int id)
        {
            var category = await _unitOfWork.Repositoy<CategoryOfVehicle>().GetByIdAsync(id);
            if (category == null)
                return NotFound(new ApiResponse(404, "Category not found."));

            _unitOfWork.Repositoy<CategoryOfVehicle>().Delete(category);
            var result = await _unitOfWork.CompleteAsync();

            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Error deleting category."));

            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Category deleted successfully.",
                    StatusCode = StatusCodes.Status200OK,
                    Body = null 
                }
            };

            return Ok(response);
        }
    }
}