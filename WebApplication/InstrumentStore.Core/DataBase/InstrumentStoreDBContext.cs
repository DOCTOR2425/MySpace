using InstrumentStore.Domain.Contracts.Products;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.DataBase
{
    public class InstrumentStoreDBContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {//DESKTOP-0MK8KC9
            optionsBuilder.UseSqlServer(@$"Server=WSA-195-74-BY;Database=MySpaceDB;
				Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;");
            //.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
        }//WSA-195-74-BY

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

            modelBuilder.Entity<ProductCard>()
                .HasKey(c => c.ProductId);
        }

        public required DbSet<Brand> Brand { get; set; }
        public required DbSet<CartItem> CartItem { get; set; }
        public required DbSet<City> City { get; set; }
        public required DbSet<Comment> Comment { get; set; }
        public required DbSet<Country> Country { get; set; }
        public required DbSet<DeliveryMethod> DeliveryMethod { get; set; }
        public required DbSet<Image> Image { get; set; }
        public required DbSet<PaidOrder> PaidOrder { get; set; }
        public required DbSet<PaidOrderItem> PaidOrderItem { get; set; }
        public required DbSet<Product> Product { get; set; }
        public required DbSet<ProductCategory> ProductCategory { get; set; }
        public required DbSet<ProductComparisonItem> ProductComparisonItem { get; set; }
        public required DbSet<ProductProperty> ProductProperty { get; set; }
        public required DbSet<ProductPropertyValue> ProductPropertyValue { get; set; }
        public required DbSet<User> User { get; set; }
        public required DbSet<DeliveryAddress> DeliveryAddress { get; set; }
        public required DbSet<UserRegistrInfo> UserRegistrInfo { get; set; }
    }
}
