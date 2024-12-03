namespace InstrumentStore.Domain.DataBase.Models
{
    public class Instrument
    {
        public int InstrumentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public byte[] Image { get; set; }
        public int Type { get; set; }
        public int CountryId { get; set; }
        public int SupplierId { get; set; }

        public Instrument(int instrumentId, string name,
            string description, decimal price, int quantity,
            byte[] image, int type, int countryId, int supplierId)
        {
            InstrumentId = instrumentId;
            Name = name;
            Description = description;
            Price = price;
            Quantity = quantity;
            Image = image;
            Type = type;
            CountryId = countryId;
            SupplierId = supplierId;
        }

        public Instrument(string name, string description,
            decimal price, int quantity, byte[] image,
            int type, int countryId, int supplierId)
        {
            Name = name;
            Description = description;
            Price = price;
            Quantity = quantity;
            Image = image;
            Type = type;
            CountryId = countryId;
            SupplierId = supplierId;
        }
    }
}
