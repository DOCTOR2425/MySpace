namespace InstrumentStore.Domain.Contracts.Filters
{
    public record CategoryFilters(
        RangePropertyForFilter[] RangePropertyForFilters,
        CollectionPropertyForFilter[] CollectionPropertyForFilters);
}
