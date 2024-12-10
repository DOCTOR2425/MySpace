namespace InstrumentStore.Domain.Contracts.Products
{
    public record ProductCard(
        Guid ProductId,
        string Name,
        decimal Price,
        int Quantity,
        string Image,
        string ProductType,
        string Brand,
        string Country);
}
