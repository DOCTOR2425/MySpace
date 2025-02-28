using Microsoft.AspNetCore.Http;

namespace InstrumentStore.Domain.Contracts.Products
{
	public record ProductRequest(
		string Name,
		string Description,
		decimal Price,
		int Quantity,
		List<IFormFile> Images,
		Guid ProductCategoryId,
		Guid BrandId,
		Guid CountryId);
}
