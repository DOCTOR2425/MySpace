namespace InstrumentStore.Domain.Contracts.Filters
{
    public record FilterRequest(
        RangeFilter[] RangeFilters,
        CollectionFilter[] CollectionFilters);

}
