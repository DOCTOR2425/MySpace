using System.ComponentModel.DataAnnotations;

namespace InstrumentStore.Domain.DataBase.Models
{
	public class ProductCategory
	{
		public Guid ProductCategoryId { get; set; }
		[MaxLength(100)]
		public string Name { get; set; } = string.Empty;
	}
}
