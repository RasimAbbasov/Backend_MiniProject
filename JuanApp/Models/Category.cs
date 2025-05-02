using System.ComponentModel.DataAnnotations;

namespace JuanApp.Models
{
    public class Category:BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public List<Product> Products { get; set; }
    }
}
