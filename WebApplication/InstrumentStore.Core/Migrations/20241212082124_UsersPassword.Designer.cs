﻿// <auto-generated />
using System;
using InstrumentStore.Domain.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace InstrumentStore.Domain.Migrations
{
    [DbContext(typeof(InstrumentStoreDBContext))]
    [Migration("20241212082124_UsersPassword")]
    partial class UsersPassword
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("InstrumentStore.Domain.DataBase.Models.Brand", b =>
                {
                    b.Property<Guid>("BrandId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("BrandId");

                    b.ToTable("Brand");
                });

            modelBuilder.Entity("InstrumentStore.Domain.DataBase.Models.Country", b =>
                {
                    b.Property<Guid>("CountryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CountryId");

                    b.ToTable("Country");
                });

            modelBuilder.Entity("InstrumentStore.Domain.DataBase.Models.DeliveryMethod", b =>
                {
                    b.Property<Guid>("DeliveryMethodId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("DeliveryMethodId");

                    b.ToTable("DeliveryMethod");
                });

            modelBuilder.Entity("InstrumentStore.Domain.DataBase.Models.OrderItem", b =>
                {
                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<Guid>("tbl_OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.HasIndex("ProductId");

                    b.HasIndex("tbl_OrderId");

                    b.ToTable("OrderItem");
                });

            modelBuilder.Entity("InstrumentStore.Domain.DataBase.Models.PaymentMethod", b =>
                {
                    b.Property<Guid>("PaymentMethodId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PaymentMethodId");

                    b.ToTable("PaymentMethod");
                });

            modelBuilder.Entity("InstrumentStore.Domain.DataBase.Models.Product", b =>
                {
                    b.Property<Guid>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BrandId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CountryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("ProductTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("ProductId");

                    b.HasIndex("BrandId");

                    b.HasIndex("CountryId");

                    b.HasIndex("ProductTypeId");

                    b.ToTable("Product");
                });

            modelBuilder.Entity("InstrumentStore.Domain.DataBase.Models.ProductArchive", b =>
                {
                    b.Property<Guid>("ProductArchiveId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BrandId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CountryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("ProductTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("ProductArchiveId");

                    b.HasIndex("BrandId");

                    b.HasIndex("CountryId");

                    b.HasIndex("ProductTypeId");

                    b.ToTable("ProductArchive");
                });

            modelBuilder.Entity("InstrumentStore.Domain.DataBase.Models.ProductType", b =>
                {
                    b.Property<Guid>("ProductTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProductTypeId");

                    b.ToTable("ProductType");
                });

            modelBuilder.Entity("InstrumentStore.Domain.DataBase.Models.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("EMail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Patronymic")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telephone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserAdressId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId");

                    b.HasIndex("UserAdressId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("InstrumentStore.Domain.DataBase.Models.UserAdress", b =>
                {
                    b.Property<Guid>("UserAdressId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Entrance")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Flat")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserAdressId");

                    b.ToTable("UserAdresses");
                });

            modelBuilder.Entity("InstrumentStore.Domain.DataBase.Models.tbl_Order", b =>
                {
                    b.Property<Guid>("tbl_OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DeliveryDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DeliveryMethodId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PaymentMethodId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("RegistrationDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("tbl_OrderId");

                    b.HasIndex("DeliveryMethodId");

                    b.HasIndex("PaymentMethodId");

                    b.HasIndex("UserId");

                    b.ToTable("tbl_Order");
                });

            modelBuilder.Entity("InstrumentStore.Domain.DataBase.Models.OrderItem", b =>
                {
                    b.HasOne("InstrumentStore.Domain.DataBase.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InstrumentStore.Domain.DataBase.Models.tbl_Order", "tbl_Order")
                        .WithMany()
                        .HasForeignKey("tbl_OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("tbl_Order");
                });

            modelBuilder.Entity("InstrumentStore.Domain.DataBase.Models.Product", b =>
                {
                    b.HasOne("InstrumentStore.Domain.DataBase.Models.Brand", "Brand")
                        .WithMany()
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InstrumentStore.Domain.DataBase.Models.Country", "Country")
                        .WithMany()
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InstrumentStore.Domain.DataBase.Models.ProductType", "ProductType")
                        .WithMany()
                        .HasForeignKey("ProductTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Brand");

                    b.Navigation("Country");

                    b.Navigation("ProductType");
                });

            modelBuilder.Entity("InstrumentStore.Domain.DataBase.Models.ProductArchive", b =>
                {
                    b.HasOne("InstrumentStore.Domain.DataBase.Models.Brand", "Brand")
                        .WithMany()
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InstrumentStore.Domain.DataBase.Models.Country", "Country")
                        .WithMany()
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InstrumentStore.Domain.DataBase.Models.ProductType", "ProductType")
                        .WithMany()
                        .HasForeignKey("ProductTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Brand");

                    b.Navigation("Country");

                    b.Navigation("ProductType");
                });

            modelBuilder.Entity("InstrumentStore.Domain.DataBase.Models.User", b =>
                {
                    b.HasOne("InstrumentStore.Domain.DataBase.Models.UserAdress", "UserAdress")
                        .WithMany()
                        .HasForeignKey("UserAdressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserAdress");
                });

            modelBuilder.Entity("InstrumentStore.Domain.DataBase.Models.tbl_Order", b =>
                {
                    b.HasOne("InstrumentStore.Domain.DataBase.Models.DeliveryMethod", "DeliveryMethod")
                        .WithMany()
                        .HasForeignKey("DeliveryMethodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InstrumentStore.Domain.DataBase.Models.PaymentMethod", "PaymentMethod")
                        .WithMany()
                        .HasForeignKey("PaymentMethodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InstrumentStore.Domain.DataBase.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DeliveryMethod");

                    b.Navigation("PaymentMethod");

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
