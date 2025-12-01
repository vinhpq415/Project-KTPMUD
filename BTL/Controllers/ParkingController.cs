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

        // POST: Ghi nhận xe vào bãi
        [HttpPost]
        public async Task<IActionResult> CheckIn(VehicleType vehicleType, string? licensePlate)
        {
            var newTicket = new ParkingTicket
            {
                VehicleType = vehicleType,
                TimeIn = DateTime.Now,
                IsActive = true
            };

            if (vehicleType == VehicleType.Bicycle)
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

            try
            {
                _context.ParkingTickets.Add(newTicket);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã nhận xe thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
            }

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