using BTL.Data;
using BTL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BTL.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Trang Doanh Thu (Revenue)
        public async Task<IActionResult> Revenue()
        {
            // Lấy danh sách lượt xe ĐÃ RA (TrangThai = 1)
            var history = await _context.LuotGuiXes
                .Include(l => l.TheXe)
                .ThenInclude(t => t.LoaiXe)
                .Where(l => l.TrangThai == 1) // 1 là đã ra (đã hoàn thành)
                .OrderByDescending(l => l.ThoiGianRa)
                .ToListAsync();

            // Tính tổng tiền (Logic đơn giản: Giờ * Giá)
            decimal totalRevenue = 0;
            foreach (var item in history)
            {
                if (item.TheXe.LoaiVe == "VangLai" && item.ThoiGianRa.HasValue)
                {
                    var hours = (item.ThoiGianRa.Value - item.ThoiGianVao).TotalHours;
                    int hoursCharged = (int)System.Math.Ceiling(hours);
                    if (hoursCharged == 0) hoursCharged = 1;

                    totalRevenue += (decimal)hoursCharged * item.TheXe.LoaiXe.GiaTheoGio;
                }
            }

            ViewBag.TotalRevenue = totalRevenue;
            return View(history);
        }

        // 2. Trang Quản lý Vé tháng (Tickets)
        public async Task<IActionResult> Tickets()
        {
            // Lấy danh sách thẻ có LoaiVe là "Thang"
            var monthlyTickets = await _context.TheXes
                .Include(t => t.NguoiDung) // Để lấy tên người đăng ký
                .Include(t => t.LoaiXe)
                .Where(t => t.LoaiVe == "Thang")
                .ToListAsync();

            return View(monthlyTickets);
        }

        // Chức năng Xóa vé (Nếu cần)
        [HttpPost]
        public async Task<IActionResult> DeleteTicket(string id)
        {
            var ticket = await _context.TheXes.FindAsync(id);
            if (ticket != null)
            {
                _context.TheXes.Remove(ticket);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Tickets));
        }
    }
}