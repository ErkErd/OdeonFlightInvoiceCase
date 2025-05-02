using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OdeonFlightInvoiceCase.Domain.Entities;

namespace OdeonFlightInvoiceCase.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ApplyUtcDateTimeConvention(modelBuilder);// date time conflict
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.ToTable("Reservations");

            // Base entity configuration
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreateDate).IsRequired();
            entity.Property(e => e.UpdateDate).IsRequired(false);
            entity.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);

            // Reservation specific configuration
            entity.Property(e => e.BookingID)
                .IsRequired();

            entity.Property(e => e.Customer)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.CarrierCode)
                .IsRequired()
                .HasMaxLength(3);//Hem IATA hem ICAO desteklemek için

            entity.Property(e => e.FlightNo)
                .IsRequired();

            entity.Property(e => e.FlightDate)
                .IsRequired();

            entity.Property(e => e.Origin)
                .IsRequired()
                .HasMaxLength(4);//Hem IATA hem ICAO desteklemek için

            entity.Property(e => e.Destination)
                .IsRequired()
                .HasMaxLength(4);//Hem IATA hem ICAO desteklemek için

            entity.Property(e => e.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            entity.Property(e => e.InvoiceNumber)
                .IsRequired(false);

            // Indexes
            entity.HasIndex(e => new { e.FlightDate, e.CarrierCode, e.FlightNo });
            entity.HasIndex(e => e.InvoiceNumber);
        });
    }
    private static void ApplyUtcDateTimeConvention(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                        value => value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(value, DateTimeKind.Utc) : value.ToUniversalTime(),
                        value => value
                    ));
                }
            }
        }
    }
} 