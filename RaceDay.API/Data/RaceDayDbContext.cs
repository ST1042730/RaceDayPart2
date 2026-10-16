
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;

namespace RaceDay.API.Data;

public class RaceDayDbContext : DbContext
{
    public RaceDayDbContext(
        DbContextOptions<RaceDayDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<EventCategory> EventCategories => Set<EventCategory>();
    public DbSet<Enrolment> Enrolments => Set<Enrolment>();
    public DbSet<Result> Results => Set<Result>();
    public DbSet<EventRoute> EventRoutes => Set<EventRoute>();
    public DbSet<EventWeather> EventWeathers => Set<EventWeather>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
modelBuilder.Entity<EventRoute>()
    .HasKey(r => r.RouteId);

modelBuilder.Entity<EventWeather>()
    .HasKey(w => w.WeatherId);


        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Event>()
            .Property(e => e.DistanceKm)
            .HasPrecision(10, 2);

        modelBuilder.Entity<EventCategory>()
            .Property(ec => ec.EntryFee)
            .HasPrecision(10, 2);

        modelBuilder.Entity<EventRoute>()
            .Property(r => r.DistanceKm)
            .HasPrecision(10, 2);

        modelBuilder.Entity<EventWeather>()
            .Property(w => w.Temperature)
            .HasPrecision(6, 2);

        modelBuilder.Entity<EventWeather>()
            .Property(w => w.WindSpeed)
            .HasPrecision(6, 2);

        modelBuilder.Entity<EventCategory>()
            .HasIndex(ec => new { ec.EventId, ec.CategoryId })
            .IsUnique();

        modelBuilder.Entity<Enrolment>()
            .HasIndex(e => new { e.ParticipantId, e.EventCategoryId })
            .IsUnique();

        modelBuilder.Entity<Result>()
            .HasIndex(r => r.EnrolmentId)
            .IsUnique();

        modelBuilder.Entity<Event>()
            .HasOne(e => e.Organiser)
            .WithMany()
            .HasForeignKey(e => e.OrganiserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Enrolment>()
            .HasOne(e => e.Participant)
            .WithMany()
            .HasForeignKey(e => e.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
