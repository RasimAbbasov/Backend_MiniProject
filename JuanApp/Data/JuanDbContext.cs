using JuanApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace JuanApp.Data
{
    public class JuanDbContext : IdentityDbContext<AppUser>
    {
        public JuanDbContext(DbContextOptions<JuanDbContext> options) : base(options)
        {

        }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<DbBasketItem> DbBasketItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(JuanDbContext).Assembly);
        }
        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreateDate = DateTime.Now;
                }
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdateDate = DateTime.Now;
                }

            }
            return base.SaveChanges();
        }
    }
}
