namespace InstrumentStore.Domain.Contracts.Filters
{
    public record CollectionPropertyForFilter(
        string PropertyName,
        string[] UniqueValues);
}
