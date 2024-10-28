using System.ComponentModel.DataAnnotations;

namespace ConsoleApp1
{
	public class CustomerEntity
	{
		[Key]
		public int CustomerID { get; set; }
		public string FIO { get; set; } = string.Empty;
		public string Telephone { get; set; } = string.Empty;
		public string EMail { get; set; } = string.Empty;

		public CustomerEntity(string fio, string telephone, string eMail)
		{
			FIO = fio;
			Telephone = telephone;
			EMail = eMail;
		}

		public CustomerEntity(int customerID, string fIO, string telephone, string eMail)
		{
			CustomerID = customerID;
			FIO = fIO;
			Telephone = telephone;
			EMail = eMail;
		}
	}
}
