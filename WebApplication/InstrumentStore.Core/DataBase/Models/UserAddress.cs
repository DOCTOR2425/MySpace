namespace InstrumentStore.Domain.DataBase.Models
{
	public class UserAddress
	{
		public Guid UserAddressId { get; set; }
		public string Street { get; set; } = string.Empty;
		public string HouseNumber { get; set; } = string.Empty;
		public string Entrance { get; set; } = string.Empty;
		public string Flat { get; set; } = string.Empty;

		public required City City { get; set; }
		public required User User { get; set; }

		public override string ToString()
		{
			return $"{City} ул.{Street} дом {HouseNumber} кв.{Flat} (подъезд {Entrance})";
		}
	}
}
