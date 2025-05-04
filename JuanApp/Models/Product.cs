using JuanApp.Areas.Manage.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace JuanApp.Models
{
    public class Product:BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int StockCount { get; set; }
        public bool InStock { get; set; }
        public bool IsTopSeller { get; set; }
        public bool IsNew { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountPercentage { get; set; }
        public int Rate { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public List<ProductImage> ProductImages { get; set; } = new();
        public List<ProductTag>? ProductTags { get; set; }

        [NotMapped]
        [AllowedType("image/png", "image/jpeg")]
        [AllowedLength(2 * 1024 * 1024)]
        public List<IFormFile>? Files { get; set; }
        [NotMapped]
        [AllowedType("image/png", "image/jpeg")]
        [AllowedLength(2 * 1024 * 1024)]
        public IFormFile? MainFile { get; set; }
    }
}
