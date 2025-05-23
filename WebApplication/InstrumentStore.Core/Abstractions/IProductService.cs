﻿using InstrumentStore.Domain.Contracts.Filters;
using InstrumentStore.Domain.Contracts.Products;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.AspNetCore.Http;

namespace InstrumentStore.Domain.Abstractions
{
	public interface IProductService
	{
		public const int PageSize = 20;

		Task<Guid> Create(Product product);
		Task<Guid> Create(CreateProductRequest productRequest, List<IFormFile> images);
		Task<Guid> ChangeArchiveStatus(Guid id, bool archiveStatus);
		Task<List<Product>> GetAll(int page);
		Task<Product> GetById(Guid id);
		Task<Guid> Update(
			Guid oldId,
			CreateProductRequest productRequest,
			List<IFormFile> images);
		Task<List<Product>> GetAllByCategory(Guid categoryId);
		Task<List<Product>> GetAllWithFilters(
			Guid categoryId,
			FilterRequest filter,
			List<Product> productsForFilter);
		Task<List<AdminProductCard>> SearchByName(string input, int page);
		Task<List<Product>> GetSimmularToProduct(Guid productId);
		Task<List<Product>> GetSpecialProductsForUser(Guid userId);
		Task<List<Product>> GetProductsByPopularity(int page);
		Task<FullProductInfoResponse> GetFullProductInfoResponse(Guid id);
		Task<ProductMinimalData> GetProductMinimalData(Guid productId);
		Task<UserProductCard> GetUserProductCard(Guid productId, Guid? userId);
		Task<List<UserProductCard>> GetUserProductCards(List<Product> products, Guid? userId);
	}
}