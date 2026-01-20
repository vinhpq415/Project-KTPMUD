USE QuanLyBaiXe;
GO

/* 1. TẠO BẢNG */
CREATE TABLE NguoiDung (
    MaNguoiDung INT IDENTITY(1,1) PRIMARY KEY,
    TenDangNhap NVARCHAR(50) NOT NULL,
    MatKhau NVARCHAR(255) NOT NULL,
    VaiTro NVARCHAR(20) NOT NULL
);

CREATE TABLE BaiXe (
    MaBaiXe INT IDENTITY(1,1) PRIMARY KEY,
    TenBaiXe NVARCHAR(100),
    TongSoCho INT
);

CREATE TABLE LoaiXe (
    MaLoaiXe INT PRIMARY KEY,
    TenLoaiXe NVARCHAR(50),
    GiaTheoGio DECIMAL(18,0)
);

CREATE TABLE TheXe (
    MaThe NVARCHAR(20) PRIMARY KEY,
    MaLoaiXe INT REFERENCES LoaiXe(MaLoaiXe),
    MaBaiXe INT REFERENCES BaiXe(MaBaiXe),
    MaNguoiDangKy INT REFERENCES NguoiDung(MaNguoiDung),
    LoaiVe NVARCHAR(20), -- 'Thang' hoac 'VangLai'
    NgayBatDau DATETIME,
    NgayHetHan DATETIME,
    BienSoDangKy NVARCHAR(20),
    TrangThai INT DEFAULT 0 -- 0: Ranh, 1: Dang gui
);

CREATE TABLE LuotGuiXe (
    MaLuotGui INT IDENTITY(1,1) PRIMARY KEY,
    MaThe NVARCHAR(20) REFERENCES TheXe(MaThe),
    MaBaiXe INT REFERENCES BaiXe(MaBaiXe),
    BienSoXe NVARCHAR(20),
    ThoiGianVao DATETIME,
    ThoiGianRa DATETIME,
    MaNhanVienVao INT REFERENCES NguoiDung(MaNguoiDung),
    MaNhanVienRa INT REFERENCES NguoiDung(MaNguoiDung),
    TrangThai INT DEFAULT 0 -- 0: Dang gui, 1: Da ra
);
GO

/* 2. THÊM DỮ LIỆU MẪU */
-- Người dùng (Admin ID=1, Bảo vệ ID=2)
INSERT INTO NguoiDung (TenDangNhap, MatKhau, VaiTro) VALUES 
('admin', '123', 'Admin'),
('baove', '123', 'Guard'),
('khach', '123', 'Customer');

-- Loại xe
INSERT INTO LoaiXe (MaLoaiXe, TenLoaiXe, GiaTheoGio) VALUES 
(1, N'Xe máy', 5000), (2, N'Ô tô', 30000), (3, N'Xe đạp', 3000);

-- Bãi xe
INSERT INTO BaiXe (TenBaiXe, TongSoCho) VALUES (N'Cổng Chính', 100);

-- Thẻ từ (Vãng lai & Tháng)
INSERT INTO TheXe (MaThe, MaLoaiXe, MaBaiXe, MaNguoiDangKy, LoaiVe, TrangThai) VALUES 
('VANG01', 1, 1, 2, 'VangLai', 0),
('VANG02', 2, 1, 2, 'VangLai', 0);

INSERT INTO TheXe (MaThe, MaLoaiXe, MaBaiXe, MaNguoiDangKy, LoaiVe, BienSoDangKy, NgayHetHan) VALUES 
('THANG01', 2, 1, 1, 'Thang', N'30H-9999', DATEADD(day, 30, GETDATE()));
GO