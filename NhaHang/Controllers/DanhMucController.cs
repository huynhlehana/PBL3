using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NhaHang.ModelFromDB;

namespace NhaHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class DanhMucController : ControllerBase
    {
        private readonly quanlynhahang dbc;

        public DanhMucController(quanlynhahang db)
        {
            dbc = db;
        }

        [HttpGet]
        [Route("/Category/List")]
        public IActionResult GetList()
        {
            return Ok(new { data = dbc.Categories.ToList() });
        }

        [HttpPost]
        [Route("/Category/Add")]
        public IActionResult ThemDanhMuc(string TenDanhMuc)
        {
            Category dm = new Category();
            dm.CategoryName = TenDanhMuc;
            dbc.Categories.Add(dm);
            dbc.SaveChanges();
            return Ok(new { message = "Thêm danh mục thành công!", data = dm });
        }

        [HttpPut]
        [Route("/Category/Update")]
        public IActionResult CapNhatDanhMuc(int ID, string TenDanhMuc)
        {
            var dm = dbc.Categories.Find(ID);
            if (dm == null)
                return NotFound(new { message = "Danh mục không tồn tại!" });
            dm.CategoryName = TenDanhMuc;
            dbc.Categories.Update(dm);
            dbc.SaveChanges();
            return Ok(new { message = "Cập nhật danh mục thành công!", data = dm });
        }

        [HttpDelete]
        [Route("/Category/Delete")]
        public IActionResult XoaDanhMuc(int ID)
        {
            var dm = dbc.Categories.Find(ID);
            if (dm == null)
                return NotFound(new { message = "Danh mục không tồn tại!" });
            dbc.Categories.Remove(dm);
            dbc.SaveChanges();
            return Ok(new { message = "Xóa danh mục thành công!", data = dm });
        }
    }
}
