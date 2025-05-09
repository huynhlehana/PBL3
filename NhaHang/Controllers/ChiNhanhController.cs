using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NhaHang.ModelFromDB;

namespace NhaHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class ChiNhanhController : ControllerBase
    {
        private readonly quanlynhahang dbc;

        public ChiNhanhController(quanlynhahang db)
        {
            dbc = db;
        }

        [HttpGet]
        [Route("/Branch/List")]
        public IActionResult GetList()
        {
            return Ok(new { data = dbc.Branches.ToList() });
        }

        [HttpPost]
        [Route("/Branch/Add")]
        public IActionResult ThemChiNhanh(string TenChiNhanh, string DiaChi, string phone, string? image)
        {
            Branch dm = new Branch();
            dm.BranchName = TenChiNhanh;
            dm.BranchAddr = DiaChi;
            dm.NumberPhone = phone;
            dm.Image = image;
            dbc.Branches.Add(dm);
            dbc.SaveChanges();
            return Ok(new { message = "Thêm chi nhánh thành công!", data = dm });
        }

        [HttpPut]
        [Route("/Branch/Update")]
        public IActionResult CapNhatThongTinChiNhanh(int ID, string TenChiNhanh, string DiaChi, string phone, string? image)
        {
            var dm = dbc.Branches.Find(ID);
            if (dm == null)
                return NotFound(new { message = "Chi nhánh không tồn tại!" });
            dm.BranchName = TenChiNhanh;
            dm.BranchAddr = DiaChi;
            dm.NumberPhone = phone;
            dm.Image = image;
            dbc.Branches.Update(dm);
            dbc.SaveChanges();
            return Ok(new { message = "Cập nhật chi nhánh thành công!", data = dm });
        }

        [HttpDelete]
        [Route("/Branch/Delete")]
        public IActionResult XoaChiNhanh(int ID)
        {
            var dm = dbc.Branches.Find(ID);
            if (dm == null)
                return NotFound(new { message = "Chi nhánh không tồn tại!" });
            dbc.Branches.Remove(dm);
            dbc.SaveChanges();
            return Ok(new { message = "Xóa chi nhánh thành công!", data = dm });
        }
    }
}
