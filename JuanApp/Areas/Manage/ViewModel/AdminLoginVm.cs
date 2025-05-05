using System.ComponentModel.DataAnnotations;

namespace JuanApp.Areas.Manage.ViewModel
{
    public class AdminLoginVm
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        public string Password { get; set; }
    }
}
