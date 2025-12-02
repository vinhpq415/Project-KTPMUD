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

            // 2. Xử lý logic
            if (newTicket.VehicleType == VehicleType.Bicycle)
            {
                var lastTicketNum = await _context.ParkingTickets
                    .Where(t => t.VehicleType == VehicleType.Bicycle)
                    .MaxAsync(t => (int?)t.TicketNumber) ?? 0;

                newTicket.TicketNumber = (lastTicketNum % 9999) + 1;
                newTicket.LicensePlate = null;
            }
            else
            {
                if (string.IsNullOrEmpty(licensePlate))
                {
                    TempData["Error"] = "Vui lòng nhập biển số xe.";
                    return RedirectToAction(nameof(Index));
                }
                newTicket.LicensePlate = licensePlate.ToUpper();
            }

            // 3. Lưu vào Database
            try
            {
                _context.ParkingTickets.Add(newTicket);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm xe thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi hệ thống: " + ex.Message;
            }

            // 4. Quay về trang danh sách (Để nhìn thấy xe vừa thêm)
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> CheckOut(int ticketId)
        {
            var ticket = await _context.ParkingTickets.FindAsync(ticketId);

            if (ticket == null || !ticket.IsActive)
            {
                TempData["Error"] = "Không tìm thấy vé xe.";
                return RedirectToAction(nameof(Index));
            }

            // Tính toán phí và cập nhật trạng thái
            ticket.TimeOut = DateTime.Now;
            ticket.IsActive = false;

            // LỖI CỦA BẠN NẰM Ở VIỆC THIẾU 2 HÀM NÀY Ở DƯỚI
            ticket.Fee = CalculateFee(ticket.VehicleType);

            _context.ParkingTickets.Update(ticket);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Xe {GetVehicleIdentifier(ticket)} đã ra. Phí thu: {ticket.Fee:N0}đ";
            return RedirectToAction(nameof(Index));
        }
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