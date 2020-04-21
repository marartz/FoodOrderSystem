﻿// <auto-generated />
using System;
using FoodOrderSystem.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FoodOrderSystem.Persistence.MSSQL.Migrations
{
    [DbContext(typeof(SystemDbContext))]
    partial class SystemDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FoodOrderSystem.Persistence.DeliveryTimeRow", b =>
                {
                    b.Property<Guid>("RestaurantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DayOfWeek")
                        .HasColumnType("int");

                    b.Property<int>("StartTime")
                        .HasColumnType("int");

                    b.Property<int>("EndTime")
                        .HasColumnType("int");

                    b.HasKey("RestaurantId", "DayOfWeek", "StartTime");

                    b.ToTable("DeliveryTimeRow");
                });

            modelBuilder.Entity("FoodOrderSystem.Persistence.DishCategoryRow", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RestaurantId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RestaurantId");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("FoodOrderSystem.Persistence.DishRow", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductInfo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RestaurantId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("RestaurantId");

                    b.ToTable("Dish");
                });

            modelBuilder.Entity("FoodOrderSystem.Persistence.DishVariantExtraRow", b =>
                {
                    b.Property<Guid>("DishId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("VariantName")
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(100)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<string>("ProductInfo")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("DishId", "VariantName", "Name");

                    b.ToTable("DishVariantExtra");
                });

            modelBuilder.Entity("FoodOrderSystem.Persistence.DishVariantRow", b =>
                {
                    b.Property<Guid>("DishId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(100)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(5, 2)");

                    b.HasKey("DishId", "Name");

                    b.ToTable("DishVariant");
                });

            modelBuilder.Entity("FoodOrderSystem.Persistence.PaymentMethodRow", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("PaymentMethod");
                });

            modelBuilder.Entity("FoodOrderSystem.Persistence.RestaurantPaymentMethodRow", b =>
                {
                    b.Property<Guid>("RestaurantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PaymentMethodId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("RestaurantId", "PaymentMethodId");

                    b.HasIndex("PaymentMethodId");

                    b.ToTable("RestaurantPaymentMethodRow");
                });

            modelBuilder.Entity("FoodOrderSystem.Persistence.RestaurantRow", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AddressCity")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AddressLine1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AddressLine2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AddressZipCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("DeliveryCosts")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<string>("Imprint")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("MinimumOrderValue")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WebSite")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Restaurant");
                });

            modelBuilder.Entity("FoodOrderSystem.Persistence.UserRow", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("PasswordSalt")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Role")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("FoodOrderSystem.Persistence.DeliveryTimeRow", b =>
                {
                    b.HasOne("FoodOrderSystem.Persistence.RestaurantRow", "Restaurant")
                        .WithMany("DeliveryTimes")
                        .HasForeignKey("RestaurantId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("FoodOrderSystem.Persistence.DishCategoryRow", b =>
                {
                    b.HasOne("FoodOrderSystem.Persistence.RestaurantRow", "Restaurant")
                        .WithMany("Categories")
                        .HasForeignKey("RestaurantId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("FoodOrderSystem.Persistence.DishRow", b =>
                {
                    b.HasOne("FoodOrderSystem.Persistence.DishCategoryRow", "Category")
                        .WithMany("Dishes")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("FoodOrderSystem.Persistence.RestaurantRow", "Restaurant")
                        .WithMany("Dishes")
                        .HasForeignKey("RestaurantId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("FoodOrderSystem.Persistence.DishVariantExtraRow", b =>
                {
                    b.HasOne("FoodOrderSystem.Persistence.DishRow", "Dish")
                        .WithMany()
                        .HasForeignKey("DishId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FoodOrderSystem.Persistence.DishVariantRow", "Variant")
                        .WithMany("Extras")
                        .HasForeignKey("DishId", "VariantName")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("FoodOrderSystem.Persistence.DishVariantRow", b =>
                {
                    b.HasOne("FoodOrderSystem.Persistence.DishRow", "Dish")
                        .WithMany("Variants")
                        .HasForeignKey("DishId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("FoodOrderSystem.Persistence.RestaurantPaymentMethodRow", b =>
                {
                    b.HasOne("FoodOrderSystem.Persistence.PaymentMethodRow", "PaymentMethod")
                        .WithMany("RestaurantPaymentMethods")
                        .HasForeignKey("PaymentMethodId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("FoodOrderSystem.Persistence.RestaurantRow", "Restaurant")
                        .WithMany("RestaurantPaymentMethods")
                        .HasForeignKey("RestaurantId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
