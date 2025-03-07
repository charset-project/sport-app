using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace sport_app_backend.Models.Account;

public class User: IdentityUser<int>
{   
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override int Id { get; set; }
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

    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
    public DateTime CreateDate { get; set; }
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
    public DateTime LastLogin { get; set; }
    public Gender Gender { get; set; }
    public byte[] ImageProfile { get; set; } = Array.Empty<byte>();
    [StringLength(500)]
    public string Bio { get; set; } = "";
    public Athlete? Athlete { get; set; }
    public Coach? Coach { get; set; }
    public TypeOfUser TypeOfUser { get; set; }
}
