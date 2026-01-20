using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL.Models
{
    [Table("BaiXe")]
    public class BaiXe
    {
        [Key]
        public int MaBaiXe { get; set; }

        [Required]
        [StringLength(100)]
        public string TenBaiXe { get; set; }

        public int TongSoCho { get; set; }
    }
}