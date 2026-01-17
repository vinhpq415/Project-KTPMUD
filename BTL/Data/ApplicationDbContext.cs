using Microsoft.EntityFrameworkCore;
using BTL.Models;

namespace BTL.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ParkingTicket> ParkingTickets { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<MonthlyTicket> MonthlyTickets { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Chỉ định rõ ràng cho thuộc tính 'Fee' của 'ParkingTicket'
            // 10: Tổng số chữ số tối đa
            // 2:  Số chữ số sau dấu phẩy

            modelBuilder.Entity<ParkingTicket>()
                .Property(p => p.Fee)
                .HasPrecision(10, 2);
        }

    } 
} 