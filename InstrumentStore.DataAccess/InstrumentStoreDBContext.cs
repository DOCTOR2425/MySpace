using InstrumentStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.DataAccess
{
	public class InstrumentStoreDBContext : DbContext
	{
		public InstrumentStoreDBContext()
		{
		}

		public InstrumentStoreDBContext(DbContextOptions options)
			: base(options)
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("Server=DESKTOP-0MK8KC9\\MSSQLSERVER01;Database=MySpaceDB;" +
				"Trusted_Connection=True;Encrypt=False;");
		}

		public DbSet<CountryEntity> Country { get; set; }
		public DbSet<CustomerEntity> Customer { get; set; }
		public DbSet<DeliveryMethodEntity> DeliveryMethod { get; set; }
		public DbSet<InstrumentEntity> Instrument { get; set; }
		public DbSet<InstrumentTypeEntity> InstrumentType { get; set; }
		public DbSet<OrderItemEntity> OrderItem { get; set; }
		public DbSet<PaymentMethodEntity> PaymentMethod { get; set; }
		public DbSet<SupplierEntity> Supplier { get; set; }
		public DbSet<tbl_OrderEntity> tbl_Order { get; set; }
	}
}
