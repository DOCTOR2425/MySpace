using Microsoft.AspNetCore.Http;

namespace InstrumentStore.Domain.Contracts.Products
{
	public record CreateProductRequest(
		string Name,
		string Description,
		decimal Price,
		int Quantity,
		List<IFormFile> Images,
		Dictionary<Guid, string> PropertyValues,
		Guid ProductCategoryId,
		Guid BrandId,
		Guid CountryId);
}
