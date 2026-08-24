using BlogManager.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogManager.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tag> Tags => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(category => category.Id);
            entity.Property(category => category.Name)
                .HasMaxLength(100)
                .UseCollation("NOCASE")
                .IsRequired();
            entity.HasIndex(category => category.Name).IsUnique();
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("Tags");
            entity.HasKey(tag => tag.Id);
            entity.Property(tag => tag.Name)
                .HasMaxLength(50)
                .UseCollation("NOCASE")
                .IsRequired();
            entity.HasIndex(tag => tag.Name).IsUnique();
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.ToTable("Posts");
            entity.HasKey(post => post.Id);
            entity.Property(post => post.Title).HasMaxLength(200).IsRequired();
            entity.Property(post => post.Content).IsRequired();
            entity.Property(post => post.Author).HasMaxLength(100).IsRequired();
            entity.Property(post => post.ViewCount).HasDefaultValue(0);
            entity.HasIndex(post => post.PublishedAt);
            entity.HasIndex(post => post.CategoryId);
            entity.HasOne(post => post.Category)
                .WithMany(category => category.Posts)
                .HasForeignKey(post => post.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasMany(post => post.Tags)
                .WithMany(tag => tag.Posts)
                .UsingEntity(join => join.ToTable("PostTags"));
        });
    }
}
