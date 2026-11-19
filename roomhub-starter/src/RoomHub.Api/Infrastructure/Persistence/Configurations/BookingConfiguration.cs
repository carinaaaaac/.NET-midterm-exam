using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Infrastructure.Persistence.Configurations;

public sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    private static readonly ValueConverter<DateTime, DateTime> UtcDateTimeConverter = new(
        value => value,
        value => DateTime.SpecifyKind(value, DateTimeKind.Utc));

    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(booking => booking.Id);
        builder.Property(booking => booking.Id).ValueGeneratedNever();
        builder.Property(booking => booking.RoomId).IsRequired();
        builder.Property(booking => booking.BookedBy).IsRequired();
        builder.Property(booking => booking.Attendees).IsRequired();
        builder.Property(booking => booking.Status).HasConversion<string>().IsRequired();
        builder.HasIndex(booking => booking.RoomId);

        builder.ComplexProperty(booking => booking.Period, period =>
        {
            period.Property(value => value.StartUtc)
                .HasColumnName("StartUtc")
                .HasConversion(UtcDateTimeConverter)
                .IsRequired();
            period.Property(value => value.EndUtc)
                .HasColumnName("EndUtc")
                .HasConversion(UtcDateTimeConverter)
                .IsRequired();
        });
    }
}
