using Microsoft.EntityFrameworkCore;
using WuyiMusic_DAL.Models.WuyiMusic_DAL.Models;

namespace WuyiMusic_DAL.Models
{
    public class WuyiMusic_DbContext : DbContext
    {
        public WuyiMusic_DbContext()
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistTrack> PlaylistTracks { get; set; }
        public DbSet<Suggestion> Suggestions { get; set; }
        public DbSet<Lyrics> Lyrics { get; set; }
        public DbSet<Queue> Queues { get; set; }
        public DbSet<QueueItem> QueueItems { get; set; }
        public DbSet<PlayHistory> PlayHistories { get; set; }
        public DbSet<UserFavoriteTrack> UserFavoriteTracks { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<ArtistFollower> ArtistFollowers { get; set; }

        public WuyiMusic_DbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=MSI\\SQLEXPRESS;Database=WuyiMusicDB;TrustServerCertificate=True;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Track>()
             .HasOne(t => t.Genre)
             .WithMany(g => g.Track)
              .HasForeignKey(t => t.GenreId)
             .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
               .HasOne(u => u.Artist)
               .WithOne(a => a.User)
               .HasForeignKey<Artist>(a => a.UserId);

            // Album configurations
            modelBuilder.Entity<Album>()
                .HasOne(a => a.Artist)
                .WithMany(artist => artist.Albums)
                .HasForeignKey(a => a.ArtistId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Album>()
                .HasMany(a => a.Tracks)
                .WithOne(t => t.Album)
                .HasForeignKey(t => t.AlbumId)
                .OnDelete(DeleteBehavior.SetNull);

            // Artist configurations
            modelBuilder.Entity<Artist>()
                .HasMany(artist => artist.Albums)
                .WithOne(album => album.Artist)
                .HasForeignKey(album => album.ArtistId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Artist>()
                .HasMany(artist => artist.Tracks)
                .WithOne(track => track.Artist)
                .HasForeignKey(track => track.ArtistId)
                .OnDelete(DeleteBehavior.SetNull);

            // Lyrics configurations
            modelBuilder.Entity<Lyrics>()
                .HasOne(l => l.Track)
                .WithMany(t => t.Lyrics)
                .HasForeignKey(l => l.TrackId)
                .OnDelete(DeleteBehavior.Cascade);

            // Playlist configurations
            modelBuilder.Entity<Playlist>()
                .HasOne(p => p.User)
                .WithMany(u => u.Playlists)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Playlist>()
                .HasMany(p => p.PlaylistTracks)
                .WithOne(pt => pt.Playlist)
                .HasForeignKey(pt => pt.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            // PlaylistTrack configurations
            modelBuilder.Entity<PlaylistTrack>()
                .HasOne(pt => pt.Playlist)
                .WithMany(p => p.PlaylistTracks)
                .HasForeignKey(pt => pt.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlaylistTrack>()
                .HasOne(pt => pt.Track)
                .WithMany(t => t.PlaylistTracks)
                .HasForeignKey(pt => pt.TrackId)
                .OnDelete(DeleteBehavior.Cascade);

            // Role configurations
            modelBuilder.Entity<Role>()
                .HasMany(r => r.UserRoles)
                .WithOne(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Role seed data
            modelBuilder.Entity<Role>().HasData(
               new Role { RoleId = Guid.Parse("d1f4eaa0-1b2c-42e8-9ff7-ff6f983ae412"), RoleName = "admin" },
               new Role { RoleId = Guid.Parse("94a3ea36-b30c-4ad8-8a9e-8262fb030fdc"), RoleName = "artist" },
               new Role { RoleId = Guid.Parse("58de85c3-30d8-4f2c-940c-002c6bb214e2"), RoleName = "user" }
            );

            // Suggestion configurations
            modelBuilder.Entity<Suggestion>()
                .HasOne(s => s.User)
                .WithMany(u => u.Suggestions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Suggestion>()
                .HasOne(s => s.Track)
                .WithMany()
                .HasForeignKey(s => s.TrackId)
                .OnDelete(DeleteBehavior.Cascade);

            // Track configurations
            modelBuilder.Entity<Track>()
                .HasOne(t => t.Album)
                .WithMany(a => a.Tracks)
                .HasForeignKey(t => t.AlbumId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Track>()
                .HasOne(t => t.Artist)
                .WithMany(artist => artist.Tracks)
                .HasForeignKey(t => t.ArtistId)
                .OnDelete(DeleteBehavior.SetNull);

            // User configurations
            modelBuilder.Entity<User>()
                .HasMany(u => u.Playlists)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserRole configurations
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // QueueItem configurations
            modelBuilder.Entity<QueueItem>()
                .HasIndex(qi => new { qi.QueueId, qi.Position })
                .IsUnique();

            modelBuilder.Entity<Queue>()
                .HasMany(q => q.QueueItems)
                .WithOne(qi => qi.Queue)
                .HasForeignKey(qi => qi.QueueId)
                .OnDelete(DeleteBehavior.Cascade);
                modelBuilder.Entity<Queue>()
                .HasOne(q => q.CurrentTrack)
                .WithMany()
                .HasForeignKey(q => q.CurrentTrackId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ArtistFollower>()
               .HasOne(af => af.User)
               .WithMany(u => u.FollowedArtists)
               .HasForeignKey(af => af.UserId)
               .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<ArtistFollower>()
                .HasOne(af => af.Artist)
                .WithMany(a => a.Followers)
                .HasForeignKey(af => af.ArtistId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}