using CategoryService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CategoryService.Infrastructure.Configs;

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("CategoryId")
            .IsRequired();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Slug)
            .HasMaxLength(150)
            .IsRequired(false);

        builder.Property(e => e.Description)
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(e => e.ParentCategoryId)
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(e => e.ImageUrl)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);

        builder.Property(e => e.IsVisible)
            .HasDefaultValue(true);

        builder.Property(e => e.Level)
            .HasDefaultValue(0);

        builder.Property(e => e.SortOrder)
            .HasDefaultValue(0);

        // Индексы
        builder.HasIndex(e => e.ParentCategoryId).HasDatabaseName("IX_Categories_ParentCategoryId");
        builder.HasIndex(e => e.Slug).HasDatabaseName("IX_Categories_Slug");
        builder.HasIndex(e => new { e.ParentCategoryId, e.Slug })
            .IsUnique()
            .HasDatabaseName("UX_Categories_Parent_Slug");

        // Навигация: Children <-> ParentCategory
        builder.HasMany(e => e.Children)
            .WithOne(e => e.ParentCategory)
            .HasForeignKey(e => e.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}