using InstrumentStore.Domain.Contracts.Filters;
using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
    public interface IProductFilterService
    {
        Task<List<Product>> GetAllWithFilters(Guid categoryId, FilterRequest filter, List<Product> productsForFilter);
    }
}