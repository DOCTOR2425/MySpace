using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
    public interface IProductPropertyService
    {
        Task<Guid> CreateProperty(ProductProperty productProperty);
        Task<Guid> CreatePropertyValue(ProductPropertyValue propertyValue);
        Task<Dictionary<string, string>> GetProductProperties(Guid productId);
    }
}