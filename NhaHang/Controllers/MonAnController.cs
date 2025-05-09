using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NhaHang.ModelFromDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace NhaHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class MonAnController : ControllerBase
    {
        private readonly quanlynhahang dbc;

        public MonAnController(quanlynhahang db)
        {
            dbc = db;
        }

        [HttpGet]
        [Route("/Food/List")]
        public IActionResult GetList()
        {
            var foods = dbc.Foods
                .Include(f => f.Category)
                .Select(f => new
                {
                    f.FoodId,
                    f.FoodName,
                    f.CategoryId,
                    CategoryName = f.Category.CategoryName,
                    f.Price,
                    f.Picture,
                })
                .ToList();
            return Ok(new { data = foods });
        }

        [HttpGet]
        [Route("/Food/Search")]
        public IActionResult TimKiemMonAn(string TenMonAn)
        {
            var ketQua = dbc.Foods
                .Include(f => f.Category)
                .Where(f => f.FoodName.Contains(TenMonAn))
                .Select(f => new
                {
                    f.FoodId,
                    f.FoodName,
                    f.CategoryId,
                    CategoryName = f.Category.CategoryName,
                    f.Price,
                    f.Picture,
                })
                .ToList();

            if (ketQua.Count == 0)
            {
                return Ok(new { message = "Trong menu không có món ăn này", data = ketQua });
            }

            return Ok(new { message = "Kết quả tìm kiếm", data = ketQua });
        }


        [HttpPost]
        [Route("/Food/Add")]
        public IActionResult ThemMonAn(string TenMonAn, int MaDanhMuc, decimal Gia, string? urlAnh)
        {
            Food dm = new Food()
            {
                FoodName = TenMonAn,
                CategoryId = MaDanhMuc,
                Price = Gia,
                Picture = urlAnh
            };

            dbc.Foods.Add(dm);
            dbc.SaveChanges();

            var ma = dbc.Foods
                .Include(f => f.Category)
                .Where(f => f.FoodId == dm.FoodId)
                .Select(f => new
                {
                    f.FoodId,
                    f.FoodName,
                    f.CategoryId,
                    CategoryName = f.Category.CategoryName,
                    f.Price,
                    f.Picture,
                })
                .FirstOrDefault();
            return Ok(new { message = "Thêm món ăn thành công!", data = ma });
        }

        [HttpPut]
        [Route("/Food/Update")]
        public IActionResult CapNhatMonAn(int ID, string? TenMonAn, int? MaDanhMuc, decimal? Gia, string? urlAnh)
        {
            var ma = dbc.Foods.Find(ID);
            if (ma == null)
                return NotFound(new { message = "Món ăn không tồn tại!" });

            ma.FoodName = TenMonAn ?? ma.FoodName;
            ma.CategoryId = MaDanhMuc ?? ma.CategoryId;
            ma.Price = Gia ?? ma.Price;
            ma.Picture = urlAnh ?? ma.Picture;

            dbc.Foods.Update(ma);
            dbc.SaveChanges();

            var mon = dbc.Foods
                .Include(f => f.Category)
                .Where(f => f.FoodId == ma.FoodId)
                .Select(f => new
                {
                    f.FoodId,
                    f.FoodName,
                    f.CategoryId,
                    CategoryName = f.Category.CategoryName,
                    f.Price,
                    f.Picture,
                })
                .FirstOrDefault();
            return Ok(new { message = "Cập nhật món ăn thành công!", data = mon });
        }

        [HttpDelete]
        [Route("/Food/Delete")]
        public IActionResult XoaMonAn(int ID)
        {
            var ma = dbc.Foods.Find(ID);
            if (ma == null)
                return NotFound(new { message = "Món ăn không tồn tại!" });

            dbc.Foods.Remove(ma);
            dbc.SaveChanges();
            return Ok(new { message = "Xóa món ăn thành công!", data = ma });
        }
    }
}
