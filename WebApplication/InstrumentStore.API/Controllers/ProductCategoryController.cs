﻿using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.ProductCategories;
using InstrumentStore.Domain.Contracts.ProductProperties;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentStore.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductCategoryController : ControllerBase
	{
		private readonly IProductCategoryService _productCategoryService;
		private readonly IMapper _mapper;

		public ProductCategoryController(
			IProductCategoryService productCategoryService,
			IMapper mapper)
		{
			_productCategoryService = productCategoryService;
			_mapper = mapper;
		}

		[HttpGet("get-all-categories")]
		public async Task<ActionResult<List<ProductCategory>>> GetAllCategories()
		{
			return await _productCategoryService.GetAll();
		}

		[HttpGet("get-properties-by-category/{id:guid}")]
		public async Task<ActionResult<List<ProductPropertyResponse>>> GetPropertiesByCategory(Guid id)
		{
			List<ProductProperty> productProperties =
				await _productCategoryService.GetProductPropertiesByCategory(id);

			List<ProductPropertyResponse> properties =
				new List<ProductPropertyResponse>(productProperties.Count);

			foreach (ProductProperty productProperty in productProperties)
				properties.Add(_mapper.Map<ProductPropertyResponse>(productProperty));

			return Ok(properties);
		}

		[HttpGet("get-top-categories-by-sales")]
		public async Task<IActionResult> GetTopCategoriesBySales()
		{
			return Ok(await _productCategoryService.GetCategoriesBySales());
		}

		[Authorize(Roles = "admin")]
		[HttpPost("create-category")]
		public async Task<ActionResult<Guid>> CreateCategory(
			[FromBody] ProductCategoryCreateRequest request)
		{
			return Ok(await _productCategoryService.Create(request));
		}

		[Authorize(Roles = "admin")]
		[HttpGet("get-categories-for-admin")]
		public async Task<IActionResult> GetProductCategoriesForAdmin()
		{
			return Ok(await _productCategoryService.GetCategoriesForAdmin());
		}

		[Authorize(Roles = "admin")]
		[HttpPost("change-visibility-status/{categoryId:guid}")]
		public async Task<IActionResult> ChangeVisibilityStatus(
			Guid categoryId,
			[FromQuery] bool visibilityStatus)
		{
			return Ok(await _productCategoryService.ChangeVisibilityStatus(categoryId, visibilityStatus));
		}

		[Authorize(Roles = "admin")]
		[HttpGet("get-category-for-update/{categoryId:guid}")]
		public async Task<ActionResult<ProductCategoryDTOUpdate>> GetProductCategoryForAdminById(Guid categoryId)
		{
			return Ok(await _productCategoryService.GetProductCategoryResponseById(categoryId));
		}

		[Authorize(Roles = "admin")]
		[HttpPost("update-category/{categoryId:guid}")]
		public async Task<ActionResult<Guid>> UpdateCategory(
			Guid categoryId,
			[FromBody] ProductCategoryDTOUpdate request)
		{
			return Ok(await _productCategoryService.Update(categoryId, request));
		}
	}
}
