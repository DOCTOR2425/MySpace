using System.ComponentModel.DataAnnotations;

namespace InstrumentStore.Domain.DataBase.Models
{
	public class ProductProperty
	{
		public Guid ProductPropertyId { get; set; }
		[MaxLength(50)]
		public string Name { get; set; } = string.Empty;
		public bool IsRanged { get; set; } = false;

		public ProductCategory ProductCategory { get; set; }
	}
}
