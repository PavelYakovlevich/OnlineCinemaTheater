using Media.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Media.Data.Context;

public class MediaServiceDbContext : DbContext
{
	public MediaServiceDbContext(DbContextOptions<MediaServiceDbContext> options) 
		: base(options)
	{
	}

	public DbSet<MediaInfo> MediaInfos { get; set; }

	public DbSet<MediaContent> MediaContents { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<MediaInfo>();

		modelBuilder.Entity<MediaContent>().HasKey(_ => new { _.Id, _.MediaId });
        modelBuilder.Entity<MediaContent>()
			.HasOne<MediaInfo>()
            .WithMany()
            .HasForeignKey(c => c.MediaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
