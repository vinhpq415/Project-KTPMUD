using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL.Models
{
    [Table("LuotGuiXe")]
    public class LuotGuiXe
    {
        [Key]
        public int MaLuotGui { get; set; }

        public string MaThe { get; set; }
        [ForeignKey("MaThe")]
        public virtual TheXe TheXe { get; set; }

        public int MaBaiXe { get; set; }
        [ForeignKey("MaBaiXe")]
        public virtual BaiXe BaiXe { get; set; }

        public string BienSoXe { get; set; }
        public DateTime ThoiGianVao { get; set; }
        public DateTime? ThoiGianRa { get; set; }

        public int MaNhanVienVao { get; set; }
        [ForeignKey("MaNhanVienVao")]
        public virtual NguoiDung NhanVienVao { get; set; }

        public int? MaNhanVienRa { get; set; }
        [ForeignKey("MaNhanVienRa")]
        public virtual NguoiDung NhanVienRa { get; set; }

        // [SỬA LỖI QUAN TRỌNG]: Đổi từ byte sang int để khớp với SQL
        public int TrangThai { get; set; }
    }
}