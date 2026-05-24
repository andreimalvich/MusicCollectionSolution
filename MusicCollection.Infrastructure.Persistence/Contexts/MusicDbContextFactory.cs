using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MusicCollection.Infrastructure.Persistence.Contexts;

internal class MusicDbContextFactory : IDesignTimeDbContextFactory<MusicDbContext>
{
    public MusicDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MusicDbContext>();
        var connectionString = @"Server=(localdb)\mssqllocaldb;Database=MusicCDDB;Trusted_Connection=True;";
        optionsBuilder.UseSqlServer(connectionString);
        return new MusicDbContext(optionsBuilder.Options);
    }
}
