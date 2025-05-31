using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace HoopStats.Models;

[Index(nameof(Email), IsUnique = true)]
[Index(nameof(Username), IsUnique = true)]
public class User
{
    public int Id { get; set; }

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsAdmin { get; set; } = false;

    public void SetPassword(string password)
    {
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
    }
}