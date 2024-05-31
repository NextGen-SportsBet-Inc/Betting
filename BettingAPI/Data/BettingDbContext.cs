using Microsoft.EntityFrameworkCore;
using SportBetInc.Models;

public class BettingDbContext(DbContextOptions<BettingDbContext> options) : DbContext(options)
{
    public DbSet<Bet> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bet>()
            .HasIndex(a => a.UserId)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }

}

