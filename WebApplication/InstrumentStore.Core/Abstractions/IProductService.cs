using InstrumentStore.Domain.Contracts.Filters;
using InstrumentStore.Domain.Contracts.Products;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.AspNetCore.Http;

namespace InstrumentStore.Domain.Abstractions
{
    public interface IProductService
    {
        public const int pageSize = 5;

        Task<Guid> Create(Product product);
        Task<Guid> Create(CreateProductRequest productRequest, List<IFormFile> images);
        Task<Guid> ChangeArchiveStatus(Guid id, bool archiveStatus);
        Task<List<Product>> GetAll(int page);
        Task<Product> GetById(Guid id);
        Task<Guid> Update(
            Guid oldId,
            CreateProductRequest productRequest,
            List<IFormFile> images);
        Task<List<Product>> GetAllByCategory(string categoryName, int page);
        Task<List<Product>> GetAllWithFilters(
            string categoryName,
            FilterRequest filter,
            List<Product> productsForFilter,
            int page);
        Task<List<ProductCard>> SearchByName(string input, int package);
    }
}