using Bogus;
using DotnetAuthYetAnother.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DotnetAuthYetAnother.Api.Data;


public class FormulaOneDbContext : IdentityDbContext
{
    public FormulaOneDbContext(DbContextOptions<FormulaOneDbContext> options) : base(options)
    {
    }

    public DbSet<TeamModel> TeamModels { get; set; }
    public DbSet<RefreshTokens> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var id = 1;

        var teams = new Faker<TeamModel>()
                            .RuleFor(m => m.Id, f => id++)
                            .RuleFor(m => m.Name, f => f.Company.CompanyName())
                            .RuleFor(m => m.Country, f => f.Address.Country())
                            .RuleFor(m => m.TeamPrinciple, f => f.Company.CatchPhrase());

        builder
            .Entity<IdentityUserLogin<string>>()
            .HasNoKey();

        builder
            .Entity<IdentityUserClaim<string>>()
            .HasNoKey();

        builder
            .Entity<IdentityUserRole<string>>()
            .HasNoKey();

        builder
            .Entity<IdentityUserToken<string>>()
            .HasNoKey();

        builder
            .Entity<TeamModel>()
            .HasData(teams.Generate(10000));
    }
}