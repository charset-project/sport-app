using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sport_app_backend.Models.Login_Sinup;

public class Login
{   [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id {get; set;}
    [Required]
    [StringLength(11)]
    //start with 09 and should be 12 digit
    [RegularExpression(@"^09\d{11}$")]
    public string PhoneNumber {get; set;}="";
    public int Code {get; set;}
    public string Role {get; set;}="";


}
