namespace InstrumentStore.Domain.DataBase.Models
{
	public class Image
	{
		public Guid ImageId { get; set; }
		public string Name { get; set; } = string.Empty;

		public required Product Product { get; set; }
	}
}
