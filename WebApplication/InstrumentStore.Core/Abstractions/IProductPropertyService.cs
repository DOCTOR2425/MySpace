using InstrumentStore.Domain.Contracts.Filters;
using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
    public interface IProductPropertyService
    {
        Task<Guid> CreateProperty(ProductProperty productProperty);
        Task<Guid> CreatePropertyValue(ProductPropertyValue propertyValue);
        Task<Dictionary<string, string>> GetProductProperties(Guid productId);
        Task<CategoryFilters> GetCategoryFilters(Guid categoryId);
        Task<RangePropertyForFilter[]> GetRangeProperties(Guid categoryId);
        Task<List<ProductPropertyValue>> GetValuesByCategoryId(Guid categoryId);
        Task<ProductProperty> GetById(Guid id);
        Task DeleteProperiesValuesByProductId(Guid productId);
        Task<Guid> DeleteById(Guid propertyId);


    }
}