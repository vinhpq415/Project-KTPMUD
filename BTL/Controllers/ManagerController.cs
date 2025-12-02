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

        // GET: /Manager/Revenue (Trang Doanh thu)
        public async Task<IActionResult> Revenue()
        {
            // 1. Lấy tất cả xe ĐÃ RA (IsActive = false) để tính tiền
            var tickets = await _context.ParkingTickets
                .Where(t => t.IsActive == false && t.Fee != null)
                .OrderByDescending(t => t.TimeOut) // Xe mới ra hiện lên đầu
                .ToListAsync();

            // 2. Tính tổng doanh thu
            decimal totalRevenue = tickets.Sum(t => t.Fee ?? 0);

            // 3. Gửi tổng tiền sang View để hiển thị
            ViewBag.TotalRevenue = totalRevenue;

            return View(tickets);
        }
        [HttpGet]
        public async Task<IActionResult> Tickets()
        {
            // Lấy danh sách, sắp xếp: Chưa duyệt lên đầu, rồi đến ngày đăng ký mới nhất
            var tickets = await _context.MonthlyTickets
                .OrderBy(t => t.IsApproved) // False (0) lên trước True (1)
                .ThenByDescending(t => t.ExpirationDate)
                .ToListAsync();

            return View(tickets);
        }

        // 2. Duyệt vé (Kích hoạt)
        [HttpPost]
        public async Task<IActionResult> ApproveTicket(int id)
        {
            var ticket = await _context.MonthlyTickets.FindAsync(id);
            if (ticket != null)
            {
                ticket.IsApproved = true; // Chuyển thành Đã duyệt
                // Gia hạn thêm 30 ngày tính từ hôm nay (ngày duyệt)
                ticket.ExpirationDate = DateTime.Now.AddDays(30);

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã duyệt vé cho xe {ticket.LicensePlate} thành công!";
            }
            return RedirectToAction(nameof(Tickets));
        }

        // 3. Xóa vé (Từ chối)
        [HttpPost]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var ticket = await _context.MonthlyTickets.FindAsync(id);
            if (ticket != null)
            {
                _context.MonthlyTickets.Remove(ticket);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã xóa vé thành công.";
            }
            return RedirectToAction(nameof(Tickets));
        }
    }
}