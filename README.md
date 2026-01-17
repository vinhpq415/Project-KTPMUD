# Hệ Thống Quản Lý Bãi Đỗ Xe Thông Minh

Hệ thống quản lý bãi đỗ xe được xây dựng trên nền tảng **ASP.NET Core MVC**, giúp tự động hóa quy trình kiểm soát xe ra vào, tính phí gửi xe và quản lý vé tháng một cách chính xác và hiệu quả.

## Tính năng
### 1. Phân Quyền Người Dùng
Hệ thống hỗ trợ 3 nhóm người dùng riêng biệt:
- **Quản lý (Admin):** Xem báo cáo doanh thu, Duyệt vé tháng, Quản lý toàn bộ hệ thống.
- **Bảo vệ (Guard):** Thực hiện thao tác Soát vé (Cho xe vào/ra), xử lý sự cố tại cổng.
- **Khách hàng (Customer):** Đăng ký tài khoản, Đăng ký vé tháng online, Xem trạng thái vé.
### 2. Quản Lý Ra / Vào (Check-in & Check-out)
- **Check-in:**
  - Nhập biển số xe (tự động chuẩn hóa chữ in hoa).
  - Tự động cảnh báo nếu biển số xe này **đang gửi** trong bãi (tránh trùng lặp).
  - Tự động cấp số vé cho xe đạp (không cần biển số).
- **Check-out:**
  - Tự động tính tiền dựa trên loại xe.
  - **Tự động nhận diện Vé Tháng:** Nếu xe có vé tháng còn hạn -> **Miễn phí (0đ)** và hiển thị số ngày còn lại.
### 3. Bảng Giá Cước Tự Động
- Xe đạp: 3k/ngày - 45k/tháng
- Xe máy: 5k/ngày - 70k/tháng
- Xe máy điện: 5k/ngày - 100k/tháng
- Xe ô tô: 15k/ngày - 3000k/tháng
### 4. Quản Lý Doanh Thu & Vé Tháng
- **Thống kê:** Báo cáo tổng doanh thu thực tế và lịch sử chi tiết từng lượt xe.
- **Duyệt vé:** Khách hàng đăng ký -> Admin kiểm tra và bấm "Duyệt" -> Vé mới có hiệu lực.

## Công Nghệ Sử Dụng (Tech Stack)

* **Core Framework:** ASP.NET Core 8.0 (MVC Pattern)
* **Database:** SQL Server (Entity Framework Core - Code First)
* **Frontend:** Razor Views, Bootstrap 5, JavaScript (ES6)
* **Tools:** Visual Studio 2022

## Installation
1.  Clone hoặc tải source code về máy.
2.  Mở file `appsettings.json`, kiểm tra chuỗi kết nối (Connection String). Mặc định dự án dùng LocalDB:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ParkingDb;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
    ```
### Bước 3: Khởi tạo Cơ sở dữ liệu (Migration)
1.  Mở Visual Studio.
2.  Vào menu **Tools** > **NuGet Package Manager** > **Package Manager Console**.
3.  Chạy lệnh sau để tạo database và các bảng:
    ```powershell
    Update-Database
    ```
### Bước 4: Tạo tài khoản Đăng nhập (Seed Data)
*Lưu ý: Hệ thống chưa có trang đăng ký cho Admin/Bảo vệ, bạn cần tạo tay trong Database lần đầu.*

1.  Trong Visual Studio, mở **View** > **SQL Server Object Explorer**.
2.  Đi tới: `(localdb)\MSSQLLocalDB` > `Databases` > `ParkingDb` > `Tables` > `dbo.Users`.
3.  Click chuột phải chọn **View Data**.
4.  Nhập thủ công 3 dòng dữ liệu sau (bỏ trống cột Id):
- Username điền lần lượt **admin**, **baove**, **khach**
- Password: Cả 3 hàng đều điền 123
- Roll: điền lần lượt **Admin**, **Guard**, **Customer**
### Bước 5: Chạy dự án
Nhấn **F5** 
## 📖 Quick Guide

### 1. Dành cho Admin
- Đăng nhập: `admin` / `123`.
- Vào menu **Doanh thu** để xem tiền.
- Vào menu **Duyệt vé tháng** để kích hoạt vé cho khách.

### 2. Dành cho Bảo vệ
- Đăng nhập: `baove` / `123`.
- Vào menu **Quản lý Ra/Vào**.
- Nhập biển số xe và bấm "Xác nhận vào".
- Bấm nút "Cho ra" để tính tiền.

### 3. Dành cho Khách hàng
- Đăng nhập: `khach` / `123`.
- Vào menu **Đăng ký vé tháng**.
- Chọn loại xe, nhập biển số và gửi yêu cầu.

## ⚠️ Khắc Phục Lỗi Thường Gặp

**1. Lỗi "HTTP Error 500.30 - ANCM In-Process Handler Load Failure"**
- Thử chạy lệnh `dotnet run` trong Terminal để xem lỗi chi tiết. Thường do sai phiên bản .NET SDK.

**2. Lỗi SSL (Not Secure)**
- Chạy lệnh sau trong CMD (Admin) để tin cậy chứng chỉ HTTPS:
  ```bash
  dotnet dev-certs https --trust
**3. Không lưu được xe vào Database**
- Kiểm tra lại xem bạn đã chạy `Update-Database` chưa.
- Đảm bảo form nhập liệu trong file View có thuộc tính `name="..."`.
