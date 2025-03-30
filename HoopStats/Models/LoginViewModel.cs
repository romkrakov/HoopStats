using System.ComponentModel.DataAnnotations;

namespace HoopStats.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "נא להזין שם משתמש")]
        public string Username { get; set; }

        [Required(ErrorMessage = "נא להזין סיסמה")]
        public string Password { get; set; }
    }
}