using Microsoft.EntityFrameworkCore;
using NhaHang.ModelFromDB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
//builder.WebHost
       //.ConfigureKestrel(options =>
       //{
       //    options.ListenAnyIP(5000);
       //});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Quản Lý Tổng"));
    options.AddPolicy("Management", policy => policy.RequireRole("Quản Lý Tổng", "Quản Lý Chi Nhánh"));
    options.AddPolicy("AllStaff", policy => policy.RequireRole("Quản Lý Tổng", "Quản Lý Chi Nhánh", "Nhân Viên"));
    options.AddPolicy("Everyone", policy => policy.RequireRole("Quản Lý Tổng", "Quản Lý Chi Nhánh", "Nhân Viên", "Khách Hàng"));
});

IConfigurationRoot cf = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
builder.Services.AddDbContext<quanlynhahang>(opt => opt.UseSqlServer(cf.GetConnectionString("cnn")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
