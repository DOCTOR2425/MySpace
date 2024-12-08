﻿namespace InstrumentStore.Domain.Contracts.Products
{
    public record ProductCard(
        Guid ProductId,
        string Name,
        decimal Price,
        int Quantity,
        byte[] Image,
        string ProductType,
        string Brand,
        string Country);
}
