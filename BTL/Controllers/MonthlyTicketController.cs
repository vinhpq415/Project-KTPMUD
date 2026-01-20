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

        // GET: Đăng ký vé tháng
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Xử lý đăng ký
        [HttpPost]
        public async Task<IActionResult> Register(string licensePlate, int vehicleType)
        {
            // 1. Kiểm tra đăng nhập
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy thông tin User từ DB mới
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.TenDangNhap == username);
            if (user == null) return RedirectToAction("Login", "Account");

            // 2. Tạo mã thẻ tự động
            string maThe = "THANG_" + new Random().Next(1000, 9999);

            // 3. Tạo đối tượng TheXe mới
            var newTicket = new TheXe
            {
                MaThe = maThe,
                MaLoaiXe = vehicleType,
                MaBaiXe = 1,            // Mặc định bãi số 1
                MaNguoiDangKy = user.MaNguoiDung,
                BienSoDangKy = licensePlate,
                LoaiVe = "Thang",
                NgayBatDau = DateTime.Now,
                NgayHetHan = DateTime.Now.AddDays(30),
                TrangThai = 0
            };

            try
            {
                _context.TheXes.Add(newTicket);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đăng ký vé tháng thành công! Mã thẻ của bạn là: " + maThe;
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                return View();
            }
        }
    }
}