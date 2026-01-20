/* =========================
   1. TẠO DATABASE
   ========================= */
USE master;
GO

IF DB_ID('QuanLyBaiXe') IS NOT NULL
BEGIN
    ALTER DATABASE QuanLyBaiXe SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuanLyBaiXe;
END
GO

CREATE DATABASE QuanLyBaiXe;
GO

USE QuanLyBaiXe;
GO
/* Bảng bãi xe */
CREATE TABLE BaiXe (
    MaBaiXe INT IDENTITY PRIMARY KEY,
    TenBaiXe NVARCHAR(100) NOT NULL,
    TongSoCho INT NOT NULL CHECK (TongSoCho > 0)
);

/* Bảng loại xe */
CREATE TABLE LoaiXe (
    MaLoaiXe INT PRIMARY KEY,
    TenLoaiXe NVARCHAR(50) NOT NULL,
    GiaTheoGio INT NOT NULL
);

/* Bảng người dùng */
CREATE TABLE NguoiDung (
    MaNguoiDung INT IDENTITY PRIMARY KEY,
    TenDangNhap VARCHAR(50) UNIQUE NOT NULL,
    MatKhau VARCHAR(255) NOT NULL,
    VaiTro NVARCHAR(20) NOT NULL
);


/* Bảng thẻ xe ( vãng lai + tháng) */
CREATE TABLE TheXe (
    MaThe VARCHAR(20) PRIMARY KEY,
    MaLoaiXe INT NOT NULL,
    MaBaiXe INT NOT NULL,
    MaNguoiDangKy INT NOT NULL,

    LoaiVe NVARCHAR(20) NOT NULL DEFAULT N'VangLai',
    NgayBatDau DATE NULL,
    NgayHetHan DATE NULL,
    BienSoDangKy NVARCHAR(20) NULL,

    TrangThai TINYINT DEFAULT 0, -- 0: rảnh | 1: đang gửi

    CONSTRAINT FK_TheXe_LoaiXe FOREIGN KEY (MaLoaiXe) REFERENCES LoaiXe(MaLoaiXe),
    CONSTRAINT FK_TheXe_BaiXe FOREIGN KEY (MaBaiXe) REFERENCES BaiXe(MaBaiXe),
    CONSTRAINT FK_TheXe_NguoiDung FOREIGN KEY (MaNguoiDangKy) REFERENCES NguoiDung(MaNguoiDung)
);
/* Bảng lượt gửi xe */
CREATE TABLE LuotGuiXe (
    MaLuotGui INT IDENTITY PRIMARY KEY,
    MaThe VARCHAR(20) NOT NULL,
    MaBaiXe INT NOT NULL,
    BienSoXe NVARCHAR(20) NOT NULL,

    ThoiGianVao DATETIME DEFAULT GETDATE(),
    ThoiGianRa DATETIME NULL,

    MaNhanVienVao INT NOT NULL,
    MaNhanVienRa INT NULL,

    TrangThai TINYINT DEFAULT 0, -- 0: đang gửi | 1: đã ra

    CONSTRAINT FK_LGX_TheXe FOREIGN KEY (MaThe) REFERENCES TheXe(MaThe),
    CONSTRAINT FK_LGX_BaiXe FOREIGN KEY (MaBaiXe) REFERENCES BaiXe(MaBaiXe),
    CONSTRAINT FK_LGX_NV_VAO FOREIGN KEY (MaNhanVienVao) REFERENCES NguoiDung(MaNguoiDung),
    CONSTRAINT FK_LGX_NV_RA FOREIGN KEY (MaNhanVienRa) REFERENCES NguoiDung(MaNguoiDung)
);
/* Khóa nghiệp vụ: 1 thẻ chỉ có 1 lượt đang gửi */
CREATE UNIQUE INDEX UX_TheDangGui
ON LuotGuiXe(MaThe)
WHERE TrangThai = 0;


/* Lịch sử ra vào */
CREATE TABLE LichSuRaVao (
    ID INT IDENTITY PRIMARY KEY,
    MaThe VARCHAR(20) NOT NULL,
    BienSoXe NVARCHAR(20) NOT NULL,
    ThoiGianVao DATETIME DEFAULT GETDATE(),
    ThoiGianRa DATETIME NULL,
    TrangThai TINYINT DEFAULT 0,       -- 0: đang gửi | 1: đã ra
    MaNhanVienVao INT NULL,
    MaNhanVienRa  INT NULL,
    CONSTRAINT FK_LSRV_TheXe
        FOREIGN KEY (MaThe) REFERENCES TheXe(MaThe),

    CONSTRAINT FK_LSRV_NV_VAO
        FOREIGN KEY (MaNhanVienVao) REFERENCES NguoiDung(MaNguoiDung),

    CONSTRAINT FK_LSRV_NV_RA
        FOREIGN KEY (MaNhanVienRa) REFERENCES NguoiDung(MaNguoiDung)
);
GO


/* Khóa nghiệp vụ ( 1 thẻ chỉ 1 xe đang gửi )*/
CREATE UNIQUE INDEX UX_TheDangGui
ON LichSuRaVao(MaThe)
WHERE TrangThai = 0;
GO


/* Bảng thanh toán */
CREATE TABLE ThanhToan (
    MaThanhToan INT IDENTITY PRIMARY KEY,
    IDLichSuRaVao INT NOT NULL UNIQUE,
    TongTien INT NOT NULL,
    ThoiGianThanhToan DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_ThanhToan_LSRV
    FOREIGN KEY (IDLichSuRaVao) REFERENCES LichSuRaVao(ID)
);
GO


