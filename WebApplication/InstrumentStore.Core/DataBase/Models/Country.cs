﻿namespace InstrumentStore.Domain.DataBase.Models
{
    public class Country
    {
        public Guid CountryId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}