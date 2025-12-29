using Kohlhaas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kohlhaas.Infrastructure.EntityConfiguration;

public class ProjectEntityTypeConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(p => p.Id);
        builder
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode(false);
        
        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(500);
        
        builder
            .Property(p => p.CurrentPhase)
            .IsRequired()
            .HasConversion<int>();
        
        builder
            .Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(12)
            .IsUnicode(false);

        // Probably won't change that often, so OK to have index
        builder.HasIndex(p => p.CurrentPhase);
        builder.HasIndex(p => p.Code).IsUnique();
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}

public class DocumentEntityTypeConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.HasKey(d => d.Id);
        // Shouldn't be a performance issue since these shouldn't change over the doc's lifecycle.
        builder
            .HasIndex(d => new { d.ProjectId, d.ControlNumber })
            .IsUnique();

        builder.HasIndex(d => d.ProjectId);
        builder.HasIndex(d => d.CreatedById);
        builder.HasIndex(d => d.SubmittedById);
        builder.HasIndex(d => d.Status);
        builder.HasIndex(d => d.Type);
        builder.HasIndex(d => d.Phase);
        
        builder
            .Property(d => d.ControlNumber)
            .IsRequired()
            .HasMaxLength(12)
            .IsUnicode(false);
        
        builder
            .Property(d => d.Title)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode(false);

        builder
            .Property(d => d.ProjectId)
            .IsRequired();

        builder
            .Property(d => d.Type)
            .IsRequired()
            .HasConversion<int>();
        
        builder
            .Property(d => d.Phase)
            .HasConversion<int>()
            .IsRequired();
        
        builder
            .Property(d => d.UnlockReason)
            .HasMaxLength(200);

        builder
            .Property(d => d.Status)
            .IsRequired()
            .HasConversion<int>();
        
        builder
            .Property(d => d.SubmissionNotes)
            .HasMaxLength(300);
        
        builder.Property(d => d.Content);

        builder.Property(d => d.CreatedById).IsRequired();

        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder
            .Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255)
            .IsUnicode(false);
        
        builder
            .HasIndex(u => u.Email)
            .IsUnique();
        
        builder
            .Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);
        
        builder
            .Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);
        
        builder
            .Property(u => u.Department)
            .IsRequired()
            .HasMaxLength(150);

        builder
            .Property(u => u.Role)
            .HasConversion<int>();
        
        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}

public class DocumentLinkEntityTypeConfiguration : IEntityTypeConfiguration<DocumentLink>
{
    public void Configure(EntityTypeBuilder<DocumentLink> builder)
    {
        builder.HasKey(dl => dl.Id);
        
        // to ensure we don't get duplicate links
        builder
            .HasIndex(dl => new { dl.SourceId, dl.TargetId, dl.RelationshipTypeId })
            .IsUnique();

        builder.Property(dl => dl.SourceId).IsRequired();
        builder.Property(dl => dl.TargetId).IsRequired();
        builder.Property(dl => dl.RelationshipTypeId).IsRequired();
        
        builder.HasQueryFilter(dl => !dl.IsDeleted);
    }
}

public class RelationshipTypeEntityTypeConfiguration : IEntityTypeConfiguration<RelationshipType>
{
    public void Configure(EntityTypeBuilder<RelationshipType> builder)
    {
        builder.HasKey(r => r.Id);
        
        builder
            .Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode(false);
        
        builder.HasIndex(r => r.Name).IsUnique();
        
        builder
            .Property(r => r.Description)
            .HasMaxLength(300);

        builder
            .Property(r => r.Icon)
            .HasMaxLength(50)
            .IsUnicode(false);
        
        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}

public class ProjectMemberEntityTypeConfiguration : IEntityTypeConfiguration<ProjectMember>
{
    public void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        builder.HasKey(pm => pm.Id);
        builder.HasIndex(pm => pm.ProjectId);
        
        builder
            .HasIndex(pm => new { pm.ProjectId, pm.UserId })
            .IsUnique();
        
        builder
            .Property(pm => pm.Role)
            .IsRequired()
            .HasConversion<int>();
        
        builder
            .Property(pm => pm.ProjectId)
            .IsRequired();

        builder.Property(pm => pm.UserId).IsRequired();
        
        builder.HasQueryFilter(pm => !pm.IsDeleted);
    }
}

public class DocumentReviewEntityTypeConfiguration : IEntityTypeConfiguration<DocumentReview>
{
    public void Configure(EntityTypeBuilder<DocumentReview> builder)
    {
        builder.HasKey(dr => dr.Id);
        /* From the MS note: By convention, an index is created in each property (or set of properties)
         that are used as a foreign key. */
        builder.HasIndex(dr => dr.DocumentId);
        builder.HasIndex(dr => dr.ReviewerId);
        // not unique b/c a reviewer can review a doc multiple times if it's rejected & resubmitted
        builder.HasIndex(dr => new { dr.DocumentId, dr.ReviewerId });
        builder.HasIndex(dr => dr.Decision);

        builder.Property(dr => dr.ReviewedAt).IsRequired();
        builder.Property(dr => dr.DocumentId).IsRequired();
        
        builder
            .Property(dr => dr.Comment)
            .HasMaxLength(500);
        
        builder
            .Property(dr => dr.Decision)
            .HasConversion<int>();
        
        builder.HasQueryFilter(dr => !dr.IsDeleted);
    }
}