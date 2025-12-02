using Microsoft.EntityFrameworkCore;
using BTL.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Thêm các dịch vụ (Services) vào container
builder.Services.AddControllersWithViews();

// 1. Thêm dịch vụ Session và HttpContextAccessor
builder.Services.AddControllersWithViews();
builder.Services.AddSession(); // <--- THÊM DÒNG NÀY
builder.Services.AddHttpContextAccessor(); // <--- THÊM DÒNG NÀY (Để lấy user ở file View)

// Cấu hình kết nối SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
app.UseStaticFiles();
// 2. Cấu hình HTTP request pipeline (Middleware)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Nếu bạn đang chạy chế độ HTTP (như hướng dẫn sửa launchSettings ở trên), 
// dòng UseHttpsRedirection này có thể giữ hoặc bỏ. 
// Tạm thời cứ để đó, nó không gây lỗi crash.
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthorization();

// Định tuyến mặc định (vào Home/Index đầu tiên)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// QUAN TRỌNG NHẤT: Lệnh này giữ cho server luôn chạy
app.Run();