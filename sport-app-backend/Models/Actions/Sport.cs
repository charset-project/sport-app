using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using sport_app_backend.Models.Account;

namespace sport_app_backend.Models.Actions;

public class Sport
{   [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public SportEnum sportEnum;
    public int ColoriesLost { get; set; }
    public int Duration { get; set; }
    public DateTime DateTime{ get; set; }
    public int AthleteId { get; set; }
    public required Athlete Athlete { get; set; }
    

}
