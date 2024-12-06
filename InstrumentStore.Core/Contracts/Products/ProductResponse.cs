namespace InstrumentStore.Domain.Contracts.Products
{
    public record ProductResponse(
        string Name,
        string Description,
        decimal Price,
        int Quantity,
        byte[] Image,
        string ProductType,
        string Brand,
        string Country);
}
