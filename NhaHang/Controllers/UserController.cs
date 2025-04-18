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

        [HttpGet]
        [Route("/User/ByBranch")]
        public IActionResult LayDanhSachUserTheoChiNhanh(int branchID)
        {
            var dsUser = dbc.Users
                .Where(t => t.BranchId == branchID)
                .Include(t => t.Role)
                .Include(t => t.Gender)
                .Include(t => t.Branch)
                .Select(t => new
                {
                    t.UserId,
                    t.UserName,
                    fullName = t.FirstName + " " + t.LastName,
                    t.PhoneNumber,
                    birthDay = t.BirthDay.ToString("yyyy-MM-dd"),
                    gender = t.Gender.GenderName,
                    role = t.Role.RoleName,
                    t.Picture,
                    CreateAt = t.CreateAt.Value.ToString("yyyy-MM-dd hh:mm:ss tt"),
                    t.BranchId,
                }).ToList();

            if (dsUser == null || dsUser.Count == 0)
                return NotFound(new { message = "Không tìm thấy user nào thuộc chi nhánh này!" });

            return Ok(new { data = dsUser });
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
                    gender = user.Gender.GenderName,
                    role = user.Role.RoleName,
                    user.Picture,
                    CreateAt = user.CreateAt.Value.ToString("yyyy-MM-dd hh:mm:ss tt"),
                    user.BranchId,
                }
            });
        }
    }
}
