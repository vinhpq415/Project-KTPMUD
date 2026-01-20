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
            
            var user = await _context.NguoiDungs
                .FirstOrDefaultAsync(u => u.TenDangNhap == username && u.MatKhau == password);

            if (user != null)
            {
                
                HttpContext.Session.SetString("Username", user.TenDangNhap);
                HttpContext.Session.SetString("Role", user.VaiTro); // VaiTro: Admin, Guard, Customer

                // Điều hướng theo quyền (VaiTro)
                if (user.VaiTro == "Admin") return RedirectToAction("Revenue", "Manager");
                if (user.VaiTro == "Guard") return RedirectToAction("Index", "Parking");
                if (user.VaiTro == "Customer") return RedirectToAction("Register", "MonthlyTicket");

                return RedirectToAction("Index", "Home");
            }

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

            
            var existingUser = await _context.NguoiDungs
                .FirstOrDefaultAsync(u => u.TenDangNhap == username);

            if (existingUser != null)
            {
                TempData["Error"] = "Tên tài khoản này đã tồn tại.";
                return View();
            }

            
            var newUser = new NguoiDung
            {
                TenDangNhap = username,
                MatKhau = password,
                VaiTro = "Customer" // Mặc định là Khách hàng
            };

            try
            {
                _context.NguoiDungs.Add(newUser);
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
            
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.TenDangNhap == username);

            if (user == null)
            {
                TempData["Error"] = "Tài khoản không tồn tại.";
                return View();
            }

       
            user.MatKhau = newPassword;
            _context.NguoiDungs.Update(user);
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