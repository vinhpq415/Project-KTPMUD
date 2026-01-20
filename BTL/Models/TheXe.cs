using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL.Models
{
    [Table("TheXe")]
    public class TheXe
    {
        [Key]
        public string MaThe { get; set; }

        // Thêm dấu ? để tránh lỗi Null
        public int? MaLoaiXe { get; set; }
        [ForeignKey("MaLoaiXe")]
        public virtual LoaiXe? LoaiXe { get; set; }

        public int? MaBaiXe { get; set; }
        [ForeignKey("MaBaiXe")]
        public virtual BaiXe? BaiXe { get; set; }

        public int? MaNguoiDangKy { get; set; }
        [ForeignKey("MaNguoiDangKy")]
        public virtual NguoiDung? NguoiDung { get; set; }

        public string? LoaiVe { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayHetHan { get; set; }

        // QUAN TRỌNG: Biển số có thể null (với vé vãng lai chưa dùng)
        public string? BienSoDangKy { get; set; }

        public int? TrangThai { get; set; }
    }
}