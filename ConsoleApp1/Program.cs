namespace ConsoleApp1
{
	public class Program
	{
		static void Main(string[] args)
		{
			InstrumentStoreDBContext db = new InstrumentStoreDBContext();
			db.InstrumentType.Add(new InstrumentTypeEntity("Отвёртка"));
			db.Supplier.Add(new SupplierEntity("Stayler", "+1234567", "qwer@gmail.com"));
			db.SaveChanges();
			db.Instrument.Add(new InstrumentEntity("str", "str", 10, 10, new byte[10], 1, 1, 1));
			Console.WriteLine(db.Instrument.Count());
		}
	}
}
