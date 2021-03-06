﻿using Microsoft.EntityFrameworkCore;
using SmileShop.Models;

namespace SmileShop.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>().HasKey(x => new { x.UserId, x.RoleId });


            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.Orders)
                    .IsRequired(false)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .HasConstraintName("FK_Order_User");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ProductId });

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_OrderDetail_Order");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_OrderDetail_Product");

            });

            modelBuilder.Entity<Product>(entity =>
            {

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_ProductGroup");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.Products)
                    .IsRequired(false)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .HasConstraintName("FK_Product_User");
            });

            modelBuilder.Entity<ProductGroup>(entity =>
            {

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.ProductGroups)
                    //.IsRequired(false)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .HasConstraintName("FK_ProductGroup_User");
            });

            modelBuilder.Entity<Stock>(entity =>
            {
                entity.HasKey(e => new { e.Id });

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Stock)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Stock_Product");


                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.Stock)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Stock_User");

            });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        // SmileShop DBSet
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ProductGroup> ProductGroup { get; set; }
        public DbSet<Stock> Stock { get; set; }
    }
}