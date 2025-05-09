using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NhaHang.ModelFromDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace NhaHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Management")]
    public class TableController : Controller
    {
        private readonly quanlynhahang dbc;

        public TableController(quanlynhahang db)
        {
            dbc = db;
        }

        [HttpGet]
        [Route("/Table/ByBranch")]
        public IActionResult LayDanhSachBanTheoChiNhanh(int branchID)
        {
            var dsBan = dbc.Tables
                .Where(t => t.BranchId == branchID)
                .Include(t => t.Status)
                .Include(t => t.Branch)
                .Select(t => new
                {
                    t.TableId,
                    t.TableNumber,
                    t.Capacity,
                    StatusName = t.Status.StatusName,
                    t.BranchId,
                }).ToList();

            if (dsBan == null || dsBan.Count == 0)
                return NotFound(new { message = "Không tìm thấy bàn nào thuộc chi nhánh này!" });

            return Ok(new { data = dsBan });
        }

        [HttpPost]
        [Route("/Table/Add")]
        public IActionResult ThemBan(int tableNumber, int capacity, int statusID, int branchID)
        {
            Table dm = new Table()
            {
                TableNumber = tableNumber,
                Capacity = capacity,
                StatusId = statusID,
                BranchId = branchID
            };
            dbc.Tables.Add(dm);
            dbc.SaveChanges();
            var ban = dbc.Tables
                .Include(t => t.Status)
                .Include(t => t.Branch)
                .Where(t => t.TableId == dm.TableId)
                .Select(t => new
                {
                    t.TableId,
                    t.TableNumber,
                    t.Capacity,
                    StatusName = t.Status.StatusName,
                    t.BranchId,
                })
                .FirstOrDefault();
            return Ok(new { message = "Thêm bàn thành công!", data = ban });
        }

        [HttpPut]
        [Route("/Table/Update")]
        public IActionResult CapNhatThongTinBan(int ID, int tableNumber, int capacity, int statusID, int branchID)
        {
            var dm = dbc.Tables.Find(ID);
            if (dm == null)
                return NotFound(new { message = "Bàn không tồn tại!" });
            dm.TableNumber = tableNumber;
            dm.Capacity = capacity;
            dm.StatusId = statusID;
            dm.BranchId = branchID;
            dbc.Tables.Update(dm);
            dbc.SaveChanges();
            var ban = dbc.Tables
                .Include(t => t.Status)
                .Include(t => t.Branch)
                .Where(t => t.TableId == dm.TableId)
                .Select(t => new
                {
                    t.TableId,
                    t.TableNumber,
                    t.Capacity,
                    StatusName = t.Status.StatusName,
                    t.BranchId,
                })
                .FirstOrDefault();
            return Ok(new { message = "Cập nhật thông tin bàn thành công!", data = ban });
        }

        [HttpDelete]
        [Route("/Table/Delete")]
        public IActionResult XoaBan(int ID)
        {
            var dm = dbc.Tables.Find(ID);
            if (dm == null)
                return NotFound(new { message = "Bàn không tồn tại!" });
            dbc.Tables.Remove(dm);
            dbc.SaveChanges();
            return Ok(new { message = "Xóa bàn thành công!", data = dm });
        }
    }
}
