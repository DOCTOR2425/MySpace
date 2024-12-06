namespace InstrumentStore.Domain.Contracts.Products
{
    public record ProductPage(
        string Name,
        decimal Price,
        string Description,
        int Quantity,
        byte[] Image,
        string ProductType,
        string Brand,
        string Country);
}
