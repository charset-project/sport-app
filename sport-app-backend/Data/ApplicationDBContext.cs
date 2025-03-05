using Microsoft.EntityFrameworkCore;
using sport_app_backend.Models.Account;

namespace sport_app_backend.Data;

public class ApplicationDBContext:DbContext
{

    public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions){

    }

    public DbSet<Athlete> Athlete {get; set;}
    public DbSet<Coach> Coach {get; set;}



}
