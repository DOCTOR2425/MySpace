using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace InstrumentStore.Domain.DataBase
{
	public class InstrumentStoreDBContext : DbContext
	{
		private readonly IConfiguration _config;

		public InstrumentStoreDBContext(IConfiguration config)
		{
			_config = config;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(@$"Server=DESKTOP-0MK8KC9\MSSQLSERVER01;Database=MySpaceDB;
				Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;");
		}

		public required DbSet<Brand> Brand { get; set; }
		public required DbSet<CartItem> CartItem { get; set; }
		public required DbSet<Country> Country { get; set; }
		public required DbSet<DeliveryMethod> DeliveryMethod { get; set; }
		public required DbSet<PaidOrder> PaidOrder { get; set; }
		public required DbSet<PaidOrderItem> PaidOrderItem { get; set; }
		public required DbSet<PaymentMethod> PaymentMethod { get; set; }
		public required DbSet<Product> Product { get; set; }
		public required DbSet<ProductArchive> ProductArchive { get; set; }
		public required DbSet<ProductCategory> ProductCategory { get; set; }
		public required DbSet<ProductProperty> ProductProperty { get; set; }
		public required DbSet<ProductPropertyValue> ProductPropertyValue { get; set; }
		public required DbSet<User> User { get; set; }
		public required DbSet<UserAdress> UserAdresses { get; set; }
		public required DbSet<UserRegistrInfo> UserRegistrInfos { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Product>()
				.HasOne(c => c.ProductCategory)
				.WithMany()
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<ProductProperty>()
				.HasOne(c => c.ProductCategory)
				.WithMany()
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
