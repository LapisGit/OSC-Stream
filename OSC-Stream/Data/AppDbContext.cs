using Microsoft.EntityFrameworkCore;
using OSC_Stream.Models.Accounts;
using OSC_Stream.Models.Shows;

namespace OSC_Stream.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Show> Shows => Set<Show>();
}