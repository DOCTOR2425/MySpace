namespace InstrumentStore.Domain.DataBase.Models
{
	public class DeliveryAddress
	{
		public Guid DeliveryAddressId { get; set; }
		public string Street { get; set; } = string.Empty;
		public string HouseNumber { get; set; } = string.Empty;
		public string Entrance { get; set; } = string.Empty;
		public string Flat { get; set; } = string.Empty;

		public required City City { get; set; }
		public required PaidOrder PaidOrder { get; set; }

		public override string ToString()
		{
			return $"{City.Name} ул.{Street} дом {HouseNumber} кв.{Flat} (подъезд {Entrance})";
		}
	}
}
