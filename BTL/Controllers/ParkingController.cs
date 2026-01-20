using BTL.Data;
using BTL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BTL.Controllers
{
    public class ParkingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ParkingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =============================================
        // GET: Hiển thị danh sách xe ĐANG GỬI (TrangThai = 0)
        // =============================================
        public async Task<IActionResult> Index()
        {
            var activeTickets = await _context.LuotGuiXes
                .Include(l => l.TheXe)
                .ThenInclude(t => t.LoaiXe)
                .Where(t => t.TrangThai == 0) // Lấy xe đang gửi
                .OrderByDescending(t => t.ThoiGianVao)
                .ToListAsync();

            return View(activeTickets);
        }

        // =============================================
        // POST: Ghi nhận xe VÀO bãi (CheckIn)
        // =============================================
        [HttpPost]
        public async Task<IActionResult> CheckIn(int maLoaiXe, string? bienSo)
        {
            // 1. Kiểm tra đăng nhập
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Account");

            var staff = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.TenDangNhap == username);
            if (staff == null) return RedirectToAction("Login", "Account");

            // 2. Validate dữ liệu
            if (string.IsNullOrEmpty(bienSo))
            {
                TempData["Error"] = "Vui lòng nhập biển số xe.";
                return RedirectToAction(nameof(Index));
            }

            string normalizedPlate = bienSo.ToUpper().Trim();

            // 3. Kiểm tra xe đã trong bãi chưa
            bool isAlreadyIn = await _context.LuotGuiXes
                .AnyAsync(l => l.BienSoXe == normalizedPlate && l.TrangThai == 0);

            if (isAlreadyIn)
            {
                TempData["Error"] = $"Xe {normalizedPlate} đang ở trong bãi rồi!";
                return RedirectToAction(nameof(Index));
            }

            // 4. TÌM THẺ (Logic quan trọng đã sửa lỗi Null)
            TheXe? theXeChon = null;

            // A. Tìm xem có phải Vé Tháng không?
            var veThang = await _context.TheXes
                .FirstOrDefaultAsync(t => t.BienSoDangKy == normalizedPlate
                                       && t.LoaiVe == "Thang"
                                       && t.NgayHetHan >= DateTime.Now);

            if (veThang != null)
            {
                // Kiểm tra trạng thái (Chấp nhận 0 hoặc null là rảnh)
                if (veThang.TrangThai == 1)
                {
                    TempData["Error"] = "Thẻ tháng này đang được sử dụng.";
                    return RedirectToAction(nameof(Index));
                }
                theXeChon = veThang;
            }
            else
            {
                // B. Khách Vãng Lai -> Tìm thẻ rảnh
                // [FIX LỖI NULL Ở ĐÂY]: Chấp nhận TrangThai là 0 HOẶC NULL
                var theRanh = await _context.TheXes
                    .FirstOrDefaultAsync(t => t.LoaiVe == "VangLai"
                                           && t.MaLoaiXe == maLoaiXe
                                           && (t.TrangThai == 0 || t.TrangThai == null));

                if (theRanh == null)
                {
                    TempData["Error"] = "Hết thẻ cho loại xe này (hoặc chưa tạo thẻ VangLai trong DB)!";
                    return RedirectToAction(nameof(Index));
                }
                theXeChon = theRanh;
            }

            // 5. Tạo lượt gửi
            var luotGui = new LuotGuiXe
            {
                MaThe = theXeChon.MaThe,
                MaBaiXe = 1,
                BienSoXe = normalizedPlate,
                ThoiGianVao = DateTime.Now,
                MaNhanVienVao = staff.MaNguoiDung,
                TrangThai = 0
            };

            // 6. Cập nhật thẻ thành BẬN
            theXeChon.TrangThai = 1;

            try
            {
                _context.LuotGuiXes.Add(luotGui);
                _context.TheXes.Update(theXeChon);
                await _context.SaveChangesAsync();

                string loaiVe = theXeChon.LoaiVe == "Thang" ? "Vé Tháng" : "Vãng Lai";
                TempData["SuccessMessage"] = $"Vào bến thành công! ({loaiVe})";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // =============================================
        // POST: Ghi nhận xe RA bãi (CheckOut)
        // =============================================
        [HttpPost]
        public async Task<IActionResult> CheckOut(int ticketId)
        {
            string username = HttpContext.Session.GetString("Username");
            var staff = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.TenDangNhap == username);
            if (staff == null) return RedirectToAction("Login", "Account");

            var luotGui = await _context.LuotGuiXes
                .Include(l => l.TheXe)
                .ThenInclude(t => t.LoaiXe)
                .FirstOrDefaultAsync(l => l.MaLuotGui == ticketId);

            if (luotGui == null || luotGui.TrangThai == 1)
            {
                TempData["Error"] = "Lỗi: Lượt gửi không tồn tại hoặc xe đã ra.";
                return RedirectToAction(nameof(Index));
            }

            // Tính tiền
            DateTime timeOut = DateTime.Now;
            double hours = (timeOut - luotGui.ThoiGianVao).TotalHours;
            int hoursCharged = (int)Math.Ceiling(hours);
            if (hoursCharged == 0) hoursCharged = 1;

            decimal phiThu = 0;
            string msgInfo = "";

            if (luotGui.TheXe?.LoaiVe == "Thang")
            {
                phiThu = 0;
                msgInfo = "<span class='badge bg-success'>VÉ THÁNG</span>";
            }
            else
            {
                // [FIX LỖI NULL]: Dùng toán tử ?? 0 để tránh lỗi nếu không tìm thấy giá
                decimal giaTien = luotGui.TheXe?.LoaiXe?.GiaTheoGio ?? 0;
                phiThu = (decimal)hoursCharged * giaTien;

                msgInfo = $"<br/>{hoursCharged}h x {giaTien:N0}đ = <b class='text-danger'>{phiThu:N0}đ</b>";
            }

            // Cập nhật DB
            luotGui.ThoiGianRa = timeOut;
            luotGui.MaNhanVienRa = staff.MaNguoiDung;
            luotGui.TrangThai = 1;

            // Trả thẻ (Quan trọng: Kiểm tra null)
            if (luotGui.TheXe != null)
            {
                luotGui.TheXe.TrangThai = 0; // Trả về rảnh
                _context.TheXes.Update(luotGui.TheXe);
            }

            _context.LuotGuiXes.Update(luotGui);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Xe {luotGui.BienSoXe} đã ra." + msgInfo;
            return RedirectToAction(nameof(Index));
        }
    }
}