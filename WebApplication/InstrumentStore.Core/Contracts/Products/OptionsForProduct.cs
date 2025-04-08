using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Contracts.Products
{
    public class OptionsForProduct
    {
        public List<Brand> Brands { get; set; }
        public List<Country> Countries { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }

        public OptionsForProduct()
        {
            Brands = new List<Brand>();
            Countries = new List<Country>();
            ProductCategories = new List<ProductCategory>();
        }

        public OptionsForProduct(
            List<Brand> brands,
            List<Country> countries,
            List<ProductCategory> productCategories)
        {
            Brands = brands;
            Countries = countries;
            ProductCategories = productCategories;
        }
    }
}
