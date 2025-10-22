using BTL.Enums;
using System.ComponentModel.DataAnnotations;

namespace BTL.Models
{
    public class ParkingTicket
    {
        [Key]
        public int Id { get; set; } // Khóa chính, tự động tăng

        [Required]
        public VehicleType VehicleType { get; set; } // Loại phương tiện

        // Biển số xe (có thể null cho xe đạp)
        public string? LicensePlate { get; set; }

        // Số thứ tự cho xe đạp (có thể null cho các xe khác)
        public int? TicketNumber { get; set; }

        [Required]
        public DateTime TimeIn { get; set; } // Thời gian vào

        public DateTime? TimeOut { get; set; } // Thời gian ra (null nếu xe còn trong bãi)

        public decimal? Fee { get; set; } // Phí (null nếu xe còn trong bãi)

        public bool IsActive { get; set; } = true; // Trạng thái (true = đang gửi)
    }
}