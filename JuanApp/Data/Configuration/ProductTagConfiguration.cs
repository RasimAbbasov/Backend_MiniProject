using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using JuanApp.Models;

namespace JuanApp.Data.Configuration
{
    public class ProductTagConfiguration: IEntityTypeConfiguration<ProductTag>
    {
            public void Configure(EntityTypeBuilder<ProductTag> builder)
            {
                builder.HasKey(x => new { x.ProductId, x.TagId });
            }
    }
}
