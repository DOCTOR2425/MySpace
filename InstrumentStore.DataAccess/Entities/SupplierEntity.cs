using System.ComponentModel.DataAnnotations;

namespace InstrumentStore.DataAccess.Entities
{
	public class SupplierEntity
	{
		[Key]
		public int SupplierID { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }

		public SupplierEntity(string name, string phone, string email)
		{
			Name = name;
			Phone = phone;
			Email = email;
		}

		public SupplierEntity(int supplierID, string name, string phone, string email)
		{
			SupplierID = supplierID;
			Name = name;
			Phone = phone;
			Email = email;
		}
	}
}
