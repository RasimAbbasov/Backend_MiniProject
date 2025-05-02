using System.ComponentModel.DataAnnotations;

namespace JuanApp.Models
{
    public class Tag:BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public List<ProductTag> ProductTags { get; set; }
    }
}
