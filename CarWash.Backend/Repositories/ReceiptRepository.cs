using CarWash.Backend.Data;
using CarWash.Backend.Models;
using CarWash.Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarWash.Backend.Repositories;

public class ReceiptRepository : IReceiptRepository
{
    private readonly AppDbContext _context;

    public ReceiptRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Receipt receipt)
    {
        await _context.Receipts.AddAsync(receipt);
    }

    public async Task<Receipt?> GetByBookingIdAsync(int bookingId)
    {
        return await _context.Receipts
            .FirstOrDefaultAsync(receipt => receipt.BookingId == bookingId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
