using System.ComponentModel.DataAnnotations;

namespace HoopStats.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "שם פרטי הוא שדה חובה")]
        [MinLength(2, ErrorMessage = "השם חייב להכיל לפחות 2 תווים")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "שם משפחה הוא שדה חובה")]
        [MinLength(2, ErrorMessage = "שם משפחה חייב להכיל לפחות 2 תווים")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "שם משתמש הוא שדה חובה")]
        [MinLength(4, ErrorMessage = "שם משתמש חייב להכיל לפחות 4 תווים")]
        public string Username { get; set; }
        [Required(ErrorMessage = "אימייל הוא שדה חובה")]
        [EmailAddress(ErrorMessage = "אנא הזן כתובת אימייל חוקית")]
        public string Email { get; set; }

        [Required(ErrorMessage = "יש להזין מגדר")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "סיסמה היא שדה חובה")]
        [MinLength(6, ErrorMessage = "סיסמה חייבת להכיל לפחות 6 תווים")]
        public string Password { get; set; }
    }
}