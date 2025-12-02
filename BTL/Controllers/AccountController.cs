using BTL.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Để dùng Session
using System.Linq;

namespace BTL.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiện trang đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Xử lý nút Đăng nhập
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Tìm user trong DB
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Đăng nhập thành công -> Lưu vào Session
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Role", user.Role);

                // Điều hướng tùy theo quyền
                if (user.Role == "Admin") return RedirectToAction("Revenue", "Manager"); // Trang doanh thu
                if (user.Role == "Guard") return RedirectToAction("Index", "Parking");   // Trang soát vé
                if (user.Role == "Customer") return RedirectToAction("Register", "MonthlyTicket"); // Trang đăng ký vé

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
            return View();
        }

        // Đăng xuất
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Xóa hết session
            return RedirectToAction("Login");
        }
    }
}