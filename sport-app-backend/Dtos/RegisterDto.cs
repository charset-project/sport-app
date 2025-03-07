using System.ComponentModel.DataAnnotations;

namespace sport_app_backend.Dtos;

public class RegisterDto
{
    [Required]
    [StringLength(15)]
    public required string FirstName { get; set; }
    [Required]
    [StringLength(50)]
    public required string LastName { get; set; }
    [Required]
    public DateTime BirthDate { get; set; }
    [StringLength(11)]
    [Required]
    public string PhoneNumber { get; set; }
}


