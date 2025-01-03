﻿using Microsoft.EntityFrameworkCore;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.Extensions.Configuration;

namespace InstrumentStore.Domain.DataBase
{
    public class InstrumentStoreDBContext : DbContext
    {
		private IConfiguration _config;

		public InstrumentStoreDBContext(IConfiguration config)
		{
			_config = config;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@$"Server=WSA-195-74-BY;Database=MySpaceDB;
				Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;");
        }

        public required DbSet<Brand> Brand { get; set; }
        public required DbSet<Country> Country { get; set; }
        public required DbSet<User> User { get; set; }
        public required DbSet<UserRegistrInfo> UserRegistrInfos { get; set; }
        public required DbSet<UserAdress> UserAdresses { get; set; }
        public required DbSet<DeliveryMethod> DeliveryMethod { get; set; }
        public required DbSet<PaidOrderItem> PaidOrderItem { get; set; }
        public required DbSet<PaymentMethod> PaymentMethod { get; set; }
        public required DbSet<Product> Product { get; set; }
        public required DbSet<ProductType> ProductType { get; set; }
        public required DbSet<PaidOrder> PaidOrder { get; set; }
        public required DbSet<ProductArchive> ProductArchive { get; set; }
        public required DbSet<CartItem> CartItem { get; set; }
    }
}
