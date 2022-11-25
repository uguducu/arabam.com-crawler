using crawler.Models;
using Microsoft.EntityFrameworkCore;

namespace crawler.Context;

public class AppDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            @"",
            (e) => { e.EnableRetryOnFailure(); });
        optionsBuilder.EnableSensitiveDataLogging();
    }
    
    public DbSet<Cars> Cars { get; set; }
}