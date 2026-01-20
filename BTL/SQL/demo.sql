/*PHẦN 3. THÊM DỮ LIỆU DEMO */

USE QuanLyBaiXe;
GO


/* DỮ LIỆU DEMO BẢNG LoaiXe */

-- Thêm các loại phương tiện phổ biến trong bãi xe
INSERT INTO LoaiXe (MaLoaiXe, TenLoaiXe, GiaTheoGio)
VALUES
(1, N'Xe máy', 5000),
(2, N'Ô tô', 30000);


/* Dữ liệu demo bảng bãi xe */
INSERT INTO BaiXe (TenBaiXe, TongSoCho)
VALUES
(N'Bãi A', 100),
(N'Bãi B', 50);

/* Dữ liệu demo thẻ vé vãng lai ( xe máy) */
INSERT INTO TheXe
(MaThe, MaLoaiXe, MaBaiXe, MaNguoiDangKy, LoaiVe)
VALUES
('VANG01', 1, 1, 2, N'VangLai'),
('VANG02', 1, 1, 2, N'VangLai');

/* Dữ liệu demo thẻ tháng (ô tô) */
INSERT INTO TheXe
(MaThe, MaLoaiXe, MaBaiXe, MaNguoiDangKy, LoaiVe,
 NgayBatDau, NgayHetHan, BienSoDangKy)
VALUES
('THANG01', 2, 2, 1, N'Thang',
 GETDATE(), DATEADD(MONTH, 1, GETDATE()), N'30H-99999');

 /* Dữ liệu demo lượt gửi xe vãng lai */
 INSERT INTO LuotGuiXe
(MaThe, MaBaiXe, BienSoXe, MaNhanVienVao)
VALUES
('VANG01', 1, N'29A-12345', 2);

/* Dữ liệu demo lượt gửi xe tháng */
INSERT INTO LuotGuiXe
(MaThe, MaBaiXe, BienSoXe, MaNhanVienVao)
VALUES
('THANG01', 2, N'30H-99999', 3);

/* Xe vãng lai ra ( cập nhật trạng thái) */
UPDATE LuotGuiXe
SET ThoiGianRa = DATEADD(HOUR, 2, ThoiGianVao),
    TrangThai = 1,
    MaNhanVienRa = 2
WHERE MaThe = 'VANG01'
  AND TrangThai = 0;




