﻿using Microsoft.EntityFrameworkCore;
using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.DataBase
{
	public class InstrumentStoreDBContext : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
            optionsBuilder.UseSqlServer(@"Server=WSA-195-74-BY;Database=MySpaceDB;
				Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;");
        }

        public DbSet<Brand> Brand { get; set; }
        public DbSet<Country> Country { get; set; }
		public DbSet<User> User { get; set; }
		public DbSet<UserRegistrInfo> UserRegistrInfos { get; set; }
		public DbSet<UserAdress> UserAdresses { get; set; }
		public DbSet<DeliveryMethod> DeliveryMethod { get; set; }
		public DbSet<OrderItem> OrderItem { get; set; }
		public DbSet<PaymentMethod> PaymentMethod { get; set; }
		public DbSet<Product> Product { get; set; }
		public DbSet<ProductType> ProductType { get; set; }
		public DbSet<tbl_Order> tbl_Order { get; set; }
		public DbSet<ProductArchive> ProductArchive { get; set; }
	}
}
