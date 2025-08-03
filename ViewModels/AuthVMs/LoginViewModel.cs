using System.ComponentModel.DataAnnotations;

namespace GYM_APP.ViewModels.AuthVMs
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "User type is required")]
        public string UserType { get; set; }

        public bool RememberMe { get; set; }
    }
}
