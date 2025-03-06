using InstrumentStore.Domain.Contracts.Some;

namespace InstrumentStore.Domain.Contracts.Products
{
	public class OptionsForProduct
	{
		public List<BrandResponse> Brands { get; set; }
		public List<CountryResponse> Countries { get; set; }
		public List<ProductCategoryResponse> ProductCategories { get; set; }

		public OptionsForProduct()
		{
			Brands = new List<BrandResponse>();
			Countries = new List<CountryResponse>();
			ProductCategories = new List<ProductCategoryResponse>();
		}

		public OptionsForProduct(
			List<BrandResponse> brands,
			List<CountryResponse> countries,
			List<ProductCategoryResponse> productCategories)
		{
			Brands = brands;
			Countries = countries;
			ProductCategories = productCategories;
		}
	}
}
