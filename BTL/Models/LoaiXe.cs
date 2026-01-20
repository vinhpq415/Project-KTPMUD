using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL.Models
{
    [Table("LoaiXe")]
    public class LoaiXe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MaLoaiXe { get; set; }

        public string TenLoaiXe { get; set; }

        // [SỬA LỖI]: Đổi từ int sang decimal để khớp với SQL
        public decimal GiaTheoGio { get; set; }
    }
}