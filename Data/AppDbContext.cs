using CarWash.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace CarWash.Backend.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    public DbSet<User> Users { get; set; }
}