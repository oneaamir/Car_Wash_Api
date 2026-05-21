using CarWash.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace CarWash.Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    public DbSet<User> Users { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<ServicePlan> ServicePlans { get; set; }
    public DbSet<AddOn> AddOns { get; set; }
    public DbSet<PromoCode> PromoCodes { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingAddOn> BookingAddOns { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Receipt> Receipts { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AddOn>()
            .Property(addOn => addOn.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ServicePlan>()
            .Property(servicePlan => servicePlan.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<PromoCode>()
            .Property(promoCode => promoCode.DiscountValue)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Booking>()
            .Property(booking => booking.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Payment>()
            .Property(payment => payment.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Car>()
            .HasOne(car => car.User)
            .WithMany(u => u.Cars)
            .HasForeignKey(car => car.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(booking => booking.User)
            .WithMany()
            .HasForeignKey(booking => booking.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(booking => booking.Car)
            .WithMany()
            .HasForeignKey(booking => booking.CarId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(booking => booking.AssignedWasher)
            .WithMany()
            .HasForeignKey(booking => booking.AssignedWasherId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Receipt>()
            .HasOne(receipt => receipt.Payment)
            .WithMany()
            .HasForeignKey(receipt => receipt.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
