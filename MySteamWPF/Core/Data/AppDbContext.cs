using Microsoft.EntityFrameworkCore;
using MySteamWPF.Core.Models;

namespace MySteamWPF.Core.Data;

public class AppDbContext : DbContext
{
    public DbSet<Game> Games { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<GameTag> GameTags { get; set; } = null!;
    public DbSet<UserGame> UserGames { get; set; } = null!;
    public DbSet<GameRating> GameRatings { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Composite key for many-to-many tables
        modelBuilder.Entity<GameTag>().HasKey(gt => new { gt.GameId, gt.TagId });
        modelBuilder.Entity<UserGame>().HasKey(ug => new { ug.UserId, ug.GameId });
        modelBuilder.Entity<GameRating>().HasKey(gr => new { gr.GameId, gr.UserId });
        
        // Relationships
        modelBuilder.Entity<GameRating>()
            .HasOne(gr => gr.Game)
            .WithMany(g => g.Ratings)
            .HasForeignKey(gr => gr.GameId);

        modelBuilder.Entity<GameRating>()
            .HasOne(gr => gr.User)
            .WithMany(u => u.Ratings)
            .HasForeignKey(gr => gr.UserId);

        modelBuilder.Entity<GameTag>()
            .HasOne(gt => gt.Game)
            .WithMany(g => g.GameTags)
            .HasForeignKey(gt => gt.GameId);

        modelBuilder.Entity<GameTag>()
            .HasOne(gt => gt.Tag)
            .WithMany(t => t.GameTags)
            .HasForeignKey(gt => gt.TagId);

        modelBuilder.Entity<UserGame>()
            .HasOne(ug => ug.User)
            .WithMany(u => u.UserGames)
            .HasForeignKey(ug => ug.UserId);

        modelBuilder.Entity<UserGame>()
            .HasOne(ug => ug.Game)
            .WithMany()
            .HasForeignKey(ug => ug.GameId);
        
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Game)
            .WithMany(g => g.Comments)
            .HasForeignKey(c => c.GameId);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId);
    }
}