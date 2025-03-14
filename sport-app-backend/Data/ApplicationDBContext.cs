using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using sport_app_backend.Models;
using sport_app_backend.Models.Account;
using sport_app_backend.Models.Actions;
using sport_app_backend.Models.Login_Sinup;
using sport_app_backend.Models.Payments;
using sport_app_backend.Models.Program;




namespace sport_app_backend.Data;

public class ApplicationDbContext : IdentityDbContext<User,Role,int>
{  public ApplicationDbContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {}

        protected override void OnModelCreating(ModelBuilder builder){
            base.OnModelCreating(builder); 
          
          builder.Entity<Role>().HasData(
        new Role { Id = 1, Name = "Admin", NormalizedName = "ADMIN" },
        new Role { Id = 2, Name = "Athlete", NormalizedName = "ATHLETE" },
        new Role { Id = 3, Name = "Coach", NormalizedName = "COACH"},
        new Role { Id = 4, Name = "None", NormalizedName = "NONE"});

    }

    public DbSet<Coach> Coaches { get; set; }
    public DbSet<Athlete> Athletes { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<WorkoutProgram> WorkoutPrograms { get; set; }
    public DbSet<CodeVerify> CodeVerifies { get; set; }
    public DbSet<CoachQuestion> CoachQuestions{get; set;}







}
