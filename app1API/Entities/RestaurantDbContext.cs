using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace app1API.Entities
{
    public class RestaurantDbContext: DbContext
    {
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<User> User { get; set; }

        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> dbContext): base(dbContext) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Restaurant>()
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(25);

            modelBuilder.Entity<Dish>()
                .Property(r => r.Name)
                .IsRequired();

            modelBuilder.Entity<Address>()
                .Property(x => x.City)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Address>()
                .Property(x => x.Street)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Owner>()
                .Property(x => x.Name)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(x => x.Email)
                .IsRequired();

            modelBuilder.Entity<Role>()
                .Property(x => x.Name)
                .IsRequired();
        }
    }
}
