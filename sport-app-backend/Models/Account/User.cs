using System.ComponentModel.DataAnnotations;

namespace sport_app_backend.Models.Account;

public class User
{
    [Required]
    [StringLength(15)]
    public required string FirstName { get; set; }
    [Required]
    [StringLength(50)]
    public required string LastName { get; set; }

    [Range(1300, 1402)]
    public int BirthDate { get; set; }
    [Key]
    [StringLength(11)]
    [Required]
    public required string PhoneNumber { get; set; }
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = "";
    public byte[] ImageProfile { get; set; } = Array.Empty<byte>();
    [StringLength(500)]
    public string Bio { get; set; } = "";
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
    public DateTime CreateDate { get; set; }
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
    public DateTime LastLogin { get; set; }
    public Gender Gender { get; set; }




}
