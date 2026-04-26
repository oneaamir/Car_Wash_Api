using CarWash.Backend.Models;
using CarWash.Backend.Repositories.Interfaces;
using CarWash.Backend.Services.Interfaces;

namespace CarWash.Backend.Tests;

internal class FakeBookingRepository : IBookingRepository
{
    public Booking? BookingByIdAndUser { get; set; }
    public Booking? BookingByIdAndAssignedWasher { get; set; }
    public Booking? BookingById { get; set; }
    public List<Booking> BookingsByUser { get; set; } = new();
    public List<Booking> BookingsByAssignedWasher { get; set; } = new();
    public bool SaveChangesCalled { get; private set; }

    public Task AddAsync(Booking booking)
    {
        BookingById = booking;
        return Task.CompletedTask;
    }

    public Task<List<Booking>> GetAllAsync() => Task.FromResult(new List<Booking>());

    public Task<Booking?> GetByIdAsync(int id) => Task.FromResult(BookingById);

    public Task<Booking?> GetByIdAndUserIdAsync(int id, int userId) => Task.FromResult(BookingByIdAndUser);

    public Task<Booking?> GetByIdAndAssignedWasherIdAsync(int id, int washerId) => Task.FromResult(BookingByIdAndAssignedWasher);

    public Task<List<Booking>> GetByUserIdAsync(int userId) => Task.FromResult(BookingsByUser);

    public Task<List<Booking>> GetByAssignedWasherIdAsync(int washerId) => Task.FromResult(BookingsByAssignedWasher);

    public Task SaveChangesAsync()
    {
        SaveChangesCalled = true;
        return Task.CompletedTask;
    }
}

internal class FakeReviewRepository : IReviewRepository
{
    public Review? ExistingReview { get; set; }
    public bool SaveChangesCalled { get; private set; }

    public Task AddAsync(Review review) => Task.CompletedTask;

    public Task<Review?> GetByBookingIdAndUserIdAsync(int bookingId, int userId) => Task.FromResult(ExistingReview);

    public Task<List<Review>> GetByBookingIdAsync(int bookingId) => Task.FromResult(new List<Review>());

    public Task<List<Review>> GetAllAsync() => Task.FromResult(new List<Review>());

    public Task SaveChangesAsync()
    {
        SaveChangesCalled = true;
        return Task.CompletedTask;
    }
}

internal class FakePaymentRepository : IPaymentRepository
{
    public Payment? PaymentByBookingId { get; set; }
    public Payment? PaymentById { get; set; }

    public Task AddAsync(Payment payment) => Task.CompletedTask;

    public Task<List<Payment>> GetAllAsync() => Task.FromResult(new List<Payment>());

    public Task<Payment?> GetByIdAsync(int id) => Task.FromResult(PaymentById);

    public Task<Payment?> GetByBookingIdAsync(int bookingId) => Task.FromResult(PaymentByBookingId);

    public Task SaveChangesAsync() => Task.CompletedTask;
}

internal class FakeReceiptRepository : IReceiptRepository
{
    public Receipt? ExistingReceipt { get; set; }
    public bool SaveChangesCalled { get; private set; }

    public Task AddAsync(Receipt receipt)
    {
        ExistingReceipt = receipt;
        return Task.CompletedTask;
    }

    public Task<Receipt?> GetByBookingIdAsync(int bookingId) => Task.FromResult(ExistingReceipt);

    public Task SaveChangesAsync()
    {
        SaveChangesCalled = true;
        return Task.CompletedTask;
    }
}

internal class FakeEmailService : IEmailService
{
    public List<(string ToEmail, string Subject, string Body, bool IsHtml)> SentEmails { get; } = new();

    public Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false)
    {
        SentEmails.Add((toEmail, subject, body, isHtml));
        return Task.CompletedTask;
    }
}

internal class FakeCarRepository : ICarRepository
{
    public Task AddAsync(Car car) => Task.CompletedTask;
    public Task<Car?> GetActiveByIdAsync(int id, int userId) => Task.FromResult<Car?>(null);
    public Task<List<Car>> GetActiveByUserIdAsync(int userId) => Task.FromResult(new List<Car>());
    public Task SaveChangesAsync() => Task.CompletedTask;
}

internal class FakeServicePlanRepository : IServicePlanRepository
{
    public Task AddAsync(ServicePlan servicePlan) => Task.CompletedTask;
    public Task<ServicePlan?> GetActiveByIdAsync(int id) => Task.FromResult<ServicePlan?>(null);
    public Task<List<ServicePlan>> GetAllActiveAsync() => Task.FromResult(new List<ServicePlan>());
    public Task SaveChangesAsync() => Task.CompletedTask;
}

internal class FakePromoCodeRepository : IPromoCodeRepository
{
    public Task AddAsync(PromoCode promoCode) => Task.CompletedTask;
    public Task<PromoCode?> GetActiveByIdAsync(int id) => Task.FromResult<PromoCode?>(null);
    public Task<PromoCode?> GetActiveByCodeAsync(string code) => Task.FromResult<PromoCode?>(null);
    public Task<List<PromoCode>> GetAllActiveAsync() => Task.FromResult(new List<PromoCode>());
    public Task SaveChangesAsync() => Task.CompletedTask;
}

internal class FakeAddOnRepository : IAddOnRepository
{
    public Task AddAsync(AddOn addOn) => Task.CompletedTask;
    public Task<AddOn?> GetActiveByIdAsync(int id) => Task.FromResult<AddOn?>(null);
    public Task<List<AddOn>> GetActiveByIdsAsync(List<int> ids) => Task.FromResult(new List<AddOn>());
    public Task<List<AddOn>> GetAllActiveAsync() => Task.FromResult(new List<AddOn>());
    public Task SaveChangesAsync() => Task.CompletedTask;
}
