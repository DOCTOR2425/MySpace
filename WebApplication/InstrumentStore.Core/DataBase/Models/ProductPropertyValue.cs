namespace InstrumentStore.Domain.DataBase.Models
{
	public class ProductPropertyValue
	{
		public Guid ProductPropertyValueId { get; set; }
		public string Value { get; set; } = string.Empty;

		public required Product Product { get; set; }
		public required ProductProperty ProductProperty { get; set; }
	}
}
