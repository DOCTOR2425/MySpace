namespace InstrumentStore.Domain.Contracts.Filters
{
    public record RangeFilter(
        decimal MinValue,
        decimal MaxValue,
        string Property);
}
