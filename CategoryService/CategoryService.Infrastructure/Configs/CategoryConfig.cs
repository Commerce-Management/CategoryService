using CategoryService.Core.Entities;

namespace CategoryService.Infrastructure.Configs;

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("CategoryID");

        builder.Property(e => e.Name)
            .HasMaxLength(100)
            .HasColumnType("text");

        builder.Property(e => e.Description)
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(e => e.ParentCategoryId)
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.HasMany(e => e.InverseParentCategory)
            .WithOne(e => e.ParentCategory)
            .HasForeignKey(e => e.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}