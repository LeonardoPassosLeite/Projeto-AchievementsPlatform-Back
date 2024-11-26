using AchievementsPlatform.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AchievementsPlatform.Data.Configurations
{
    public class GameFeedbackConfiguration : IEntityTypeConfiguration<GameFeedback>
    {
        public void Configure(EntityTypeBuilder<GameFeedback> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.SteamUserId).IsRequired();
            builder.Property(f => f.AppId).IsRequired();
            builder.Property(f => f.Comment).HasMaxLength(500);
            builder.Property(f => f.Rating).IsRequired();
            builder.Property(f => f.Recommend).IsRequired();
            builder.Property(f => f.CreatedAt).IsRequired();
            builder.Property(f => f.UpdatedAt).IsRequired();

            builder.HasIndex(f => new { f.SteamUserId, f.AppId }).IsUnique();
        }
    }
}