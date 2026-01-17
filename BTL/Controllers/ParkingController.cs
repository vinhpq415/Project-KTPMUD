using BTL.Data;
using BTL.Enums;
using BTL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // GET: Hiển thị danh sách xe đang gửi
        public async Task<IActionResult> Index()
        {
            var activeTickets = await _context.ParkingTickets
                                              .Where(t => t.IsActive)
                                              .OrderByDescending(t => t.TimeIn)
                                              .ToListAsync();
            return View(activeTickets);
        }

        // POST: Ghi nhận xe vào bãi (HÀM BẠN CẦN THAY THẾ)
        [HttpPost]
        public async Task<IActionResult> CheckIn(int vehicleType, string? licensePlate)
        {
            // 1. Khởi tạo đối tượng
            var newTicket = new ParkingTicket
            {
                VehicleType = (VehicleType)vehicleType, // Ép kiểu số sang Enum
                TimeIn = DateTime.Now,
                IsActive = true
            };

            // 2. Xử lý logic theo loại xe
            if (newTicket.VehicleType == VehicleType.Bicycle)
            {
                // --- LOGIC XE ĐẠP ---
                // Tìm số vé lớn nhất hiện có
                var lastTicketNum = await _context.ParkingTickets
                    .Where(t => t.VehicleType == VehicleType.Bicycle)
                    .MaxAsync(t => (int?)t.TicketNumber) ?? 0;

                newTicket.TicketNumber = (lastTicketNum % 9999) + 1;
                newTicket.LicensePlate = null;
            }
            else
            {
                // --- LOGIC XE MÁY / Ô TÔ ---

                // a. Kiểm tra nhập trống
                if (string.IsNullOrEmpty(licensePlate))
                {
                    TempData["Error"] = "Vui lòng nhập biển số xe.";
                    return RedirectToAction(nameof(Index));
                }

                // Chuẩn hóa biển số (In hoa)
                string normalizedPlate = licensePlate.ToUpper();

                // b. KIỂM TRA TRÙNG BIỂN SỐ
                // Kiểm tra xem xe này có đang ở trong bãi không (IsActive = true)
                bool isDuplicate = await _context.ParkingTickets
                    .AnyAsync(t => t.LicensePlate == normalizedPlate && t.IsActive == true);

                if (isDuplicate)
                {
                    TempData["Error"] = $"Xe biển số {normalizedPlate} đang ở trong bãi rồi! Vui lòng kiểm tra lại.";
                    return RedirectToAction(nameof(Index));
                }

                newTicket.LicensePlate = normalizedPlate;
            }

            // 3. Lưu vào Database
            try
            {
                _context.ParkingTickets.Add(newTicket);
                await _context.SaveChangesAsync();

                // Tạo thông báo chi tiết
                string info = newTicket.VehicleType == VehicleType.Bicycle
                    ? $"Vé số {newTicket.TicketNumber}"
                    : $"Biển số {newTicket.LicensePlate}";

                TempData["SuccessMessage"] = $"Đã nhận xe thành công! ({info})";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi hệ thống: " + ex.Message;
            }

            // 4. Quay về trang danh sách
            return RedirectToAction(nameof(Index));
        }

        // POST: Ghi nhận xe ra bãi và tính phí
        [HttpPost]
        public async Task<IActionResult> CheckOut(int ticketId)
        {
            var ticket = await _context.ParkingTickets.FindAsync(ticketId);

            if (ticket == null || !ticket.IsActive)
            {
                TempData["Error"] = "Không tìm thấy vé xe.";
                return RedirectToAction(nameof(Index));
            }

            // Cập nhật thông tin ra bãi
            ticket.TimeOut = DateTime.Now;
            ticket.IsActive = false;

            // --- KIỂM TRA VÉ THÁNG ---
            // Tìm xem có vé tháng nào khớp biển số + đã duyệt + còn hạn không
            var monthlyTicket = await _context.MonthlyTickets
                .FirstOrDefaultAsync(m => m.LicensePlate == ticket.LicensePlate
                                       && m.IsApproved == true
                                       && m.ExpirationDate >= DateTime.Now);

            string messageInfo = "";

            if (monthlyTicket != null)
            {
                // CÓ VÉ THÁNG -> MIỄN PHÍ
                ticket.Fee = 0;
                int daysLeft = (monthlyTicket.ExpirationDate - DateTime.Now).Days;
                messageInfo = $"<br/><span class='badge bg-success'>XE VÉ THÁNG</span> <span class='badge bg-warning text-dark'>Còn hạn: {daysLeft} ngày</span>";
            }
            else
            {
                // KHÁCH VÃNG LAI -> TÍNH TIỀN
                ticket.Fee = CalculateFee(ticket.VehicleType);
                messageInfo = $"<br/>Phí thu: <span style='font-size: 20px; font-weight: bold; color: red;'>{ticket.Fee:N0}đ</span>";
            }

            _context.ParkingTickets.Update(ticket);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Xe <b>{GetVehicleIdentifier(ticket)}</b> đã ra." + messageInfo;
            return RedirectToAction(nameof(Index));
        }

        // --- Hàm hỗ trợ ---
        private decimal CalculateFee(VehicleType vehicleType)
        {
            switch (vehicleType)
            {
                case VehicleType.Bicycle:
                    return 3000;
                case VehicleType.Motorbike:
                case VehicleType.Ebike:
                    return 5000;
                case VehicleType.Car:
                    return 15000;
                default:
                    return 0;
            }
        }

        private string GetVehicleIdentifier(ParkingTicket ticket)
        {
            if (ticket.VehicleType == VehicleType.Bicycle)
            {
                return $"Xe đạp (Vé số: {ticket.TicketNumber})";
            }
            return ticket.LicensePlate ?? "N/A";
        }
    }
}