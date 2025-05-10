using Microsoft.EntityFrameworkCore;
using Readora.DataBase.Abstractions;
using Readora.Models;

namespace Readora.DataBase;


/// <remarks>
///     dotnet tool install --global dotnet-ef
///     dotnet ef migrations add Init --project Readora.DataBase\Readora.DataBase.csproj --startup-project Readora.API\Readora.API.csproj
///     dotnet ef database update --project Readora.DataBase\Readora.DataBase.csproj --startup-project .\Readora.API\Readora.API.csproj
/// </remarks>
public class ReadoraDbContext : DbContext, IDbWriter, IDbReader, IUnitOfWork
{
    public ReadoraDbContext(DbContextOptions<ReadoraDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserCredential> UserCredentials { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<ModerationRequest> ModerationRequests { get; set; }
    public DbSet<BlockchainTransaction> BlockchainTransactions { get; set; }

    void IDbWriter.Add<TEntity>(TEntity entity)
    {
        Set<TEntity>().Entry(entity).State = EntityState.Added;
    }

    void IDbWriter.Update<TEntity>(TEntity entity)
    {
        Set<TEntity>().Entry(entity).State = EntityState.Modified;
    }

    void IDbWriter.Delete<TEntity>(TEntity entity)
    {
        Set<TEntity>().Entry(entity).State = EntityState.Deleted;
    }

    IQueryable<TEntity> IDbReader.Read<TEntity>()
    {
        return base.Set<TEntity>()
            .AsNoTracking()
            .IgnoreAutoIncludes()
            .AsQueryable();
    }

    async Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken)
    {
        var count = await base.SaveChangesAsync(cancellationToken);
        foreach (var entry in base.ChangeTracker.Entries().ToArray())
        {
            entry.State = EntityState.Detached;
        }

        return count;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>()
            .HasIndex(u => u.Name)
            .IsUnique()
            .HasFilter("\"Name\" IS NOT NULL");

        modelBuilder.Entity<Genre>()
            .HasIndex(u => u.Name)
            .IsUnique()
            .HasFilter("\"Name\" IS NOT NULL");

        modelBuilder.Entity<User>()
            .HasIndex(u => new { u.Username, u.Email })
            .IsUnique()
            .HasFilter("\"Username\" IS NOT NULL OR \"Email\" IS NOT NULL");

        modelBuilder.Entity<Book>()
            .HasIndex(u => new { u.Title })
            .IsUnique()
            .HasFilter("\"Title\" IS NOT NULL");

        modelBuilder.Entity<BlockchainTransaction>()
            .HasIndex(u => new { u.TransactionHash })
            .IsUnique()
            .HasFilter("\"TransactionHash\" IS NOT NULL");
        
        modelBuilder.Entity<BookStatus>()
            .HasIndex(u => new { u.Name })
            .IsUnique()
            .HasFilter("\"Name\" IS NOT NULL");
    }
}