namespace InstrumentStore.Domain.Models
{
	public class Instrument
	{
		public int InstrumentID { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public int Quantity { get; set; }
		public Byte[] Image { get; set; }
		public int Type { get; set; }
		public int Country { get; set; }
		public int Supplier { get; set; }

		public Instrument(int instrumentID, string name, 
			string description, decimal price, int quantity, 
			byte[] image, int instrumentType, int country, int supplier)
		{
			InstrumentID = instrumentID;
			Name = name;
			Description = description;
			Price = price;
			Quantity = quantity;
			Image = image;
			Type = instrumentType;
			Country = country;
			Supplier = supplier;
		}
	}
}
