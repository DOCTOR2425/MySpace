namespace InstrumentStore.Domain.Contracts.Filters
{
    public record RangeFilter(
        decimal ValueFrom,
        decimal ValueTo,
        string Property);
}
