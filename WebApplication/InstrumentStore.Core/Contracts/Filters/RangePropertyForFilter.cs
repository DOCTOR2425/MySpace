namespace InstrumentStore.Domain.Contracts.Filters
{
    public record RangePropertyForFilter(
        string PropertyName,
        decimal MaxValue,
        decimal MinValue);
}
