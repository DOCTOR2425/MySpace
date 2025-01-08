namespace InstrumentStore.Domain.Contracts.Products
{
    public record ProductResponse(
        ProductResponseData ProductResponseData,
        Dictionary<string, string> Properties);
}
