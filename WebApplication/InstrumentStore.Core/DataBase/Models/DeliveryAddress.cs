using System.ComponentModel.DataAnnotations;

namespace InstrumentStore.Domain.DataBase.Models
{
	public class DeliveryAddress
	{
		public Guid DeliveryAddressId { get; set; }
		[MaxLength(100)]
		public string Street { get; set; } = string.Empty;
		[MaxLength(10)]
		public string HouseNumber { get; set; } = string.Empty;
		[MaxLength(10)]
		public string Entrance { get; set; } = string.Empty;
		[MaxLength(10)]
		public string Flat { get; set; } = string.Empty;
		[MaxLength(100)]
		public required string City { get; set; }

		public required PaidOrder PaidOrder { get; set; }

		public override string ToString()
		{
			return $"{City} ул.{Street} дом {HouseNumber} кв.{Flat} (подъезд {Entrance})";
		}
	}
}
