using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using NhaHang.ModelFromDB;

namespace NhaHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly quanlynhahang dbc;

        public UserController(quanlynhahang db)
        {
            dbc = db;
        }

        [HttpPost("/User/Login")]
        public IActionResult Login(string username, string password)
        {
            var user = dbc.Users
                .Include(u => u.Gender)
                .Include(u => u.Role)
                .Include(u => u.Branch)
                .FirstOrDefault(u => u.UserName == username);

            if (user == null)
                return Unauthorized(new { message = "Tài khoản không tồn tại!" });
            if (user.Password != password)
                return Unauthorized(new { message = "Sai mật khẩu!" });

            return Ok(new
            {
                message = "Đăng nhập thành công!",
                user = new
                {
                    fullName = user.FirstName + " " + user.LastName,
                    user.PhoneNumber,
                    birthday = user.BirthDay.ToString("yyyy-MM-dd"),
                    genderName = user.Gender.GenderName,
                    roleName = user.Role.RoleName,
                    branchName = user.Branch.BranchName,
                    user.Picture,
                    CreateAt = user.CreateAt.Value.ToString("yyyy-MM-dd hh:mm:ss tt"),
                }
            });
        }
    }
}
