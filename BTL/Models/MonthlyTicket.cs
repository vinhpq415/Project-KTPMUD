using BTL.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace BTL.Models
{
    public class MonthlyTicket
    {
        [Key]
        public int Id { get; set; }

        public string CustomerName { get; set; } // Tên khách hàng

        public string PhoneNumber { get; set; }  // Số điện thoại

        public string LicensePlate { get; set; } // Biển số xe

        public VehicleType VehicleType { get; set; } // Loại xe (Xe đạp, Xe máy...)

        public DateTime ExpirationDate { get; set; } // Ngày hết hạn vé

        public bool IsApproved { get; set; } = false; // Trạng thái: false = Chưa duyệt, true = Đã duyệt
    }
}