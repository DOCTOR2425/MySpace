namespace InstrumentStore.Domain.Contracts.Products
{
    public record ProductRequest(
        string Name,
        string Description,
        decimal Price,
        int Quantity,
        byte[] Image,
        Guid ProductTypeId,
        Guid BrandId,
        Guid CountryId);
}
