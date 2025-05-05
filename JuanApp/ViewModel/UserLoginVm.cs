using System.ComponentModel.DataAnnotations;

namespace JuanApp.ViewModel
{
    public class UserLoginVm
    {
        [Required]
        public string UserNameOrEmail { get; set; }
        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
