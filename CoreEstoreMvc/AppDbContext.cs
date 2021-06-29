using CoreEstoreMvc.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CoreEstoreMvc
{
    public class AppDbContext : IdentityDbContext<User,Role, int>
    {
        public AppDbContext(DbContextOptions options):
            base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(p => (p.BaseType == typeof(AppEntity) || p.BaseType == typeof(BaseEntity) || p.BaseType == typeof(SortableBaseEntity)) && !p.IsAbstract)
                .ToList()
                .ForEach(p =>
                {
                    p.GetMethod("Build").Invoke(Activator.CreateInstance(p), new[] { builder });

                });
            builder.Entity<User>(entity => 
            {
                entity
                .HasMany(p => p.Categories)
                .WithOne(p => p.User)
                .OnDelete(DeleteBehavior.Restrict);
                entity
                .HasMany(p => p.Products)
                .WithOne(p => p.User)
                .OnDelete(DeleteBehavior.Restrict);
                entity
                .HasMany(p => p.Brands)
                .WithOne(p => p.User)
                .OnDelete(DeleteBehavior.Restrict);
                entity
                .HasMany(p => p.Banners)
                .WithOne(p => p.User)
                .OnDelete(DeleteBehavior.Restrict);
                entity
                .HasMany(p => p.Orders)
                .WithOne(p => p.User)
                .OnDelete(DeleteBehavior.Restrict);
                entity
                .HasMany(p => p.ProductComments)
                .WithOne(p => p.User)
                .OnDelete(DeleteBehavior.Restrict);
                entity
                .HasMany(p => p.ShoppingCartsItems)
                .WithOne(p => p.User)
                .OnDelete(DeleteBehavior.Restrict);
                entity
                .HasMany(p => p.Rayons)
                .WithOne(p => p.User)
                .OnDelete(DeleteBehavior.Restrict);
               

            });

            base.OnModelCreating(builder);
        }

        public virtual DbSet<Banner> Banners { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CategoryProduct> CategoryProducts { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductComment> ProductComments { get; set; }
        public virtual DbSet<ProductPicture> ProductPictures{ get; set; }
        public virtual DbSet<Rayon> Rayons { get; set; }
        public virtual DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }


    }
}
