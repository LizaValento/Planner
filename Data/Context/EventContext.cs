using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class EventContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventParticipant> EventParticipants { get; set; }

        public EventContext(DbContextOptions<EventContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id)
                    .ValueGeneratedOnAdd();
                entity.Property(u => u.FirstName);
                entity.Property(u => u.LastName);
                entity.Property(u => u.Nickname);
                entity.HasIndex(u => u.Nickname);
                entity.Property(u => u.Password);
                entity.Property(u => u.Email);
                entity.Property(u => u.Role);
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.Title);
                entity.Property(e => e.Description);
                entity.Property(e => e.Date);
                entity.Property(e => e.Location);
                entity.Property(e => e.CreatedBy);
                entity.Property(e => e.CreatedAt);
            });

            modelBuilder.Entity<EventParticipant>(entity =>
            {
                entity.HasKey(ep => ep.Id);
                entity.Property(ep => ep.Id)
                    .ValueGeneratedOnAdd();
                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(ep => ep.UserId);
                entity.HasOne<Event>()             
                    .WithMany()                      
                    .HasForeignKey(ep => ep.EventId);
                entity.Property(ep => ep.IsOrganizer);
                entity.HasIndex(ep => new { ep.EventId, ep.UserId })
                    .IsUnique();
            });
        }
    }
}
