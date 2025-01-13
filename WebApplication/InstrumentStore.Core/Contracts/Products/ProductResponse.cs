namespace InstrumentStore.Domain.Contracts.Products
{
    public record ProductResponse(
        ProductData ProductResponseData,
        Dictionary<string, string> Properties);
}
