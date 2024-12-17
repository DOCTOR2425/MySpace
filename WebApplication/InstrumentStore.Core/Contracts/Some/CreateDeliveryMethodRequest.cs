namespace InstrumentStore.Domain.Contracts.Some
{
    public record CreateDeliveryMethodRequest(
        string Name,
        decimal Price
    );
}
