using JuanApp.Areas.Manage.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JuanApp.Models
{
    public class Slider:BaseEntity
    {
            public string Subtitle { get; set; }
            public string Title { get; set; }
            public string? Image { get; set; }
            public string Description { get; set; }
            public string ButtonLink { get; set; }
            public string ButtonText { get; set; }
        public int Order { get; set; }
        [NotMapped]
        [AllowedType("image/png", "image/jpeg")]
        [AllowedLength(2 * 1024 * 1024)]
        public IFormFile File { get; set; }     

    }
}
