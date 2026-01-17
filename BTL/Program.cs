using Microsoft.EntityFrameworkCore;
using BTL.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Thêm các dịch vụ (Services) vào container
builder.Services.AddControllersWithViews();

// 1. Thêm dịch vụ Session và HttpContextAccessor
builder.Services.AddControllersWithViews();
builder.Services.AddSession(); 
builder.Services.AddHttpContextAccessor(); 

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

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthorization();

// Định tuyến mặc định (vào Home/Index đầu tiên)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();