using BTL.Data;
using BTL.Models; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore; 
using System.Linq;
using System.Threading.Tasks;
using System;

namespace BTL.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. ĐĂNG NHẬP (Login)
        // ==========================================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Tìm user trong DB (Dùng Async để không chặn luồng)
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Lưu Session
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Role", user.Role);

                // Điều hướng theo quyền
                if (user.Role == "Admin") return RedirectToAction("Revenue", "Manager");
                if (user.Role == "Guard") return RedirectToAction("Index", "Parking");
                if (user.Role == "Customer") return RedirectToAction("Register", "MonthlyTicket");

                return RedirectToAction("Index", "Home");
            }

            // Dùng TempData để hiển thị lỗi nhất quán với các hàm khác
            TempData["Error"] = "Sai tài khoản hoặc mật khẩu!";
            return View();
        }

        // ==========================================
        // 2. ĐĂNG KÝ (Register)
        // ==========================================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password, string confirmPassword)
        {
            // Kiểm tra nhập liệu
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            if (password != confirmPassword)
            {
                TempData["Error"] = "Mật khẩu xác nhận không khớp.";
                return View();
            }

            // Kiểm tra trùng tên tài khoản
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (existingUser != null)
            {
                TempData["Error"] = "Tên tài khoản này đã tồn tại.";
                return View();
            }

            // Tạo user mới
            var newUser = new User
            {
                Username = username,
                Password = password,
                Role = "Customer" // Mặc định là Khách hàng
            };

            try
            {
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                return View();
            }
        }

        // ==========================================
        // 3. QUÊN MẬT KHẨU (Forgot Password)
        // ==========================================
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string username, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                TempData["Error"] = "Tài khoản không tồn tại.";
                return View();
            }

            // Cập nhật mật khẩu mới
            user.Password = newPassword;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công! Hãy đăng nhập lại.";
            return RedirectToAction(nameof(Login));
        }

        // ==========================================
        // 4. ĐĂNG XUẤT (Logout)
        // ==========================================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}