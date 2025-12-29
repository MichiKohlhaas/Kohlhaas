using Kohlhaas.Domain.Entities;
using Kohlhaas.Infrastructure.EntityConfiguration;
using Microsoft.EntityFrameworkCore;

namespace Kohlhaas.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public DbSet<Document> Documents { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<RelationshipType> RelationshipTypes { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }
    public DbSet<DocumentReview> DocumentReviews { get; set; }
    public DbSet<DocumentLink> DocumentLinks { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
            
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new UserEntityTypeConfiguration().Configure(modelBuilder.Entity<User>());
        new DocumentEntityTypeConfiguration().Configure(modelBuilder.Entity<Document>());
        new ProjectEntityTypeConfiguration().Configure(modelBuilder.Entity<Project>());
        new ProjectMemberEntityTypeConfiguration().Configure(modelBuilder.Entity<ProjectMember>());
        new RelationshipTypeEntityTypeConfiguration().Configure(modelBuilder.Entity<RelationshipType>());
        new DocumentReviewEntityTypeConfiguration().Configure(modelBuilder.Entity<DocumentReview>());
        new DocumentLinkEntityTypeConfiguration().Configure(modelBuilder.Entity<DocumentLink>());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        // For the CreatedAt property.
        // But not every Entity inherits EntityBase which has ModifiedAt, try set datetime if it has
        var entries = ChangeTracker.Entries<IEntity>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(nameof(IEntity.CreatedAt)).CurrentValue = DateTime.UtcNow;
            }

            if (entry.State != EntityState.Modified) continue;
            var modifiedAtProperty = entry.Properties
                .FirstOrDefault(p => p.Metadata.Name == "ModifiedAt");

            modifiedAtProperty?.CurrentValue = DateTime.UtcNow;
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}
