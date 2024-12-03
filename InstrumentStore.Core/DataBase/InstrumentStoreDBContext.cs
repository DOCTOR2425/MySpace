using Microsoft.EntityFrameworkCore;
using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.DataBase
{
	public class InstrumentStoreDBContext : DbContext
	{
		public InstrumentStoreDBContext()
		{
		}

		public InstrumentStoreDBContext(DbContextOptions options)
			: base(options)
		{
            Database.EnsureCreated();
        }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("Server=WSA-195-74-BY;initial catalog=MySpaceDB;" +
				"Trusted_Connection=True;Encrypt=False;");
		}

		public DbSet<Country> Country { get; set; }
		public DbSet<Customer> Customer { get; set; }
		public DbSet<DeliveryMethod> DeliveryMethod { get; set; }
		public DbSet<Instrument> Instrument { get; set; }
		public DbSet<InstrumentType> InstrumentType { get; set; }
		public DbSet<OrderItem> OrderItem { get; set; }
		public DbSet<PaymentMethod> PaymentMethod { get; set; }
		public DbSet<Supplier> Supplier { get; set; }
		public DbSet<tbl_Order> tbl_Order { get; set; }
	}
}
