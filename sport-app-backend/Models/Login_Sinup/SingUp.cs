using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sport_app_backend.Models.Login_Sinup;

public class SingUp
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id {get; set;}
    [Required]
    [StringLength(11)]
    [RegularExpression(@"^09\d{11}$")]
    public string PhoneNumber {get; set;}="";
    public int Code {get; set;}
    public bool CodeApprove {get; set;}
    public string Role {get; set;}="";
}
