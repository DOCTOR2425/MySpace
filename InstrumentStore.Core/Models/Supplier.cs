namespace InstrumentStore.Domain.Models
{
	public class Supplier
	{
		public int SupplierID { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }

		public Supplier(int supplierID, string name, string phone, string email)
		{
			SupplierID = supplierID;
			Name = name;
			Phone = phone;
			Email = email;
		}
	}
}
