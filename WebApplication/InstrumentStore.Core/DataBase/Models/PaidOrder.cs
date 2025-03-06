using System.Diagnostics.CodeAnalysis;

namespace InstrumentStore.Domain.DataBase.Models
{
	public class PaidOrder
	{
		public Guid PaidOrderId { get; set; }
		public DateTime OrderDate { get; set; }
		public DateTime ReceiptDate { get; set; } = DateTime.MinValue;
		public string PaymentMethod { get; set; } = string.Empty;

		public required DeliveryMethod DeliveryMethod { get; set; }
		public required User User { get; set; }
	}
}
