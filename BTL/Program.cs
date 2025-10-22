using Microsoft.EntityFrameworkCore;
using BTL.Data;

var builder = WebApplication.CreateBuilder(args);

// Thêm chuỗi kết nối
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();
// ...