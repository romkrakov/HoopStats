using System.ComponentModel.DataAnnotations;

namespace HoopStats.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "שדה חובה")]
        [Display(Name = "שם משתמש")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "שדה חובה")]
        [DataType(DataType.Password)]
        [Display(Name = "סיסמה")]
        public required string Password { get; set; }
    }
}