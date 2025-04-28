using JuanApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace JuanApp.Data
{
    public class JuanDbContext: DbContext
    {
        public JuanDbContext(DbContextOptions<JuanDbContext> options) : base(options)
        {

        }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Feature> Features { get; set; }
    }
}
