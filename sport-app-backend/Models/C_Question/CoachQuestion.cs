using System.ComponentModel.DataAnnotations;
using sport_app_backend.Models.Account;

namespace sport_app_backend.Models;

public class CoachQuestion
{   [Key]
    public int Id{get; set;}
    public int UserId{get; set;}
    public User? User{get; set;}

    
}
