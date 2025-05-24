using System.ComponentModel.DataAnnotations;

namespace HoopStats.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "שדה חובה")]
        [Display(Name = "שם פרטי")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "שדה חובה")]
        [Display(Name = "שם משפחה")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "שדה חובה")]
        [Display(Name = "שם משתמש")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "שדה חובה")]
        [EmailAddress(ErrorMessage = "כתובת אימייל לא תקינה")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "שדה חובה")]
        [Display(Name = "מין")]
        public required string Gender { get; set; }

        [Required(ErrorMessage = "שדה חובה")]
        [DataType(DataType.Password)]
        [Display(Name = "סיסמה")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "שדה חובה")]
        [Compare("Password", ErrorMessage = "הסיסמאות אינן תואמות")]
        [DataType(DataType.Password)]
        [Display(Name = "אימות סיסמה")]
        public required string ConfirmPassword { get; set; }
    }
}