using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using sport_app_backend.Models.Account;
using Microsoft.EntityFrameworkCore.Design;

namespace sport_app_backend.Data;

public class ApplicationDBContext:IdentityDbContext
{

    public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions){

    }

    public DbSet<Athlete> Athlete {get; set;}
    public DbSet<Coach> Coach {get; set;}



}
