using BTL.Models;
using Microsoft.EntityFrameworkCore;

namespace BTL.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Khai báo các bảng mới
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<BaiXe> BaiXes { get; set; }
        public DbSet<LoaiXe> LoaiXes { get; set; }
        public DbSet<TheXe> TheXes { get; set; }
        public DbSet<LuotGuiXe> LuotGuiXes { get; set; }
        // public DbSet<ThanhToan> ThanhToans { get; set; } // Nếu bạn dùng bảng thanh toán
    }
}