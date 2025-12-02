using BTL.Data;
using BTL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BTL.Controllers
{
    public class MonthlyTicketController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MonthlyTicketController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Hiển thị form đăng ký (/MonthlyTicket/Register)
        [HttpGet]
        public IActionResult Register()
        {
            // Kiểm tra quyền: Chỉ tài khoản "Customer" mới được vào
            var role = HttpContext.Session.GetString("Role");
            if (role != "Customer")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // POST: Xử lý khi bấm nút "Gửi đăng ký"
        [HttpPost]
        public async Task<IActionResult> Register(MonthlyTicket ticket)
        {
            // 1. Kiểm tra biển số (Bắt buộc)
            if (string.IsNullOrEmpty(ticket.LicensePlate))
            {
                ViewBag.Error = "Vui lòng nhập biển số xe (hoặc mã định danh).";
                return View(ticket);
            }

            // 2. Kiểm tra trùng lặp: Biển số này đã có vé tháng CÒN HẠN chưa?
            bool isExist = await _context.MonthlyTickets.AnyAsync(m =>
                m.LicensePlate == ticket.LicensePlate &&
                m.ExpirationDate >= DateTime.Now);

            if (isExist)
            {
                ViewBag.Error = $"Xe biển số {ticket.LicensePlate} hiện đang có vé tháng còn hiệu lực!";
                return View(ticket);
            }

            // 3. Thiết lập mặc định
            ticket.ExpirationDate = DateTime.Now.AddMonths(1); // Mặc định đăng ký 1 tháng
            ticket.IsApproved = false; // Mặc định là CHƯA DUYỆT (Chờ Admin duyệt)

            // 4. Lưu vào Database
            try
            {
                _context.MonthlyTickets.Add(ticket);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng chờ Quản lý duyệt vé.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi hệ thống: " + ex.Message;
                return View(ticket);
            }
        }
    }
}