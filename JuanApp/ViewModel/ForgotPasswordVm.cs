using System.ComponentModel.DataAnnotations;

namespace JuanApp.ViewModel
{
    public class ForgotPasswordVm
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        
    }
}
