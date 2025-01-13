namespace InstrumentStore.Domain.Contracts.Products
{
    public record ProductData(
        Guid ProductId,
        string Name,
        string Description,
        decimal Price,
        int Quantity,
        string Image,
        string ProductCategory,
        string Brand,
        string Country);
}
