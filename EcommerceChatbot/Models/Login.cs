using System.ComponentModel.DataAnnotations;

namespace ECommerceChatbot.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }
        public bool RememberMe { get; set; } // Add this property

    }
}
