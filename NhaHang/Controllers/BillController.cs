using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using NhaHang.ModelFromDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace NhaHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Everyone")]
    public class BillController : Controller
    {
        private readonly quanlynhahang dbc;

        public BillController(quanlynhahang db)
        {
            dbc = db;
        }

        [HttpPost]
        [Route("/Bill/GetByTable")]
        public IActionResult LayHoaDonTheoBan(int tableId)
        {
            var table = dbc.Tables.Find(tableId);
            if (table == null)
                return NotFound(new { message = "Không tìm thấy bàn!" });

            if (table.StatusId != 3)
                return Ok(new { message = "Bàn chưa được sử dụng, không có hóa đơn!" });

            var existingBill = dbc.Bills
                .Include(b => b.BillItems)
                .ThenInclude(i => i.Food)
                .FirstOrDefault(b => b.TableId == tableId && b.PaidDate == null);

            if (existingBill == null)
                return StatusCode(500, new { message = "Bàn đang sử dụng nhưng không có hóa đơn! Kiểm tra dữ liệu." });

            return Ok(new
            {
                message = "Lấy hóa đơn thành công",
                billId = existingBill.BillId,
                danhSachMon = existingBill.BillItems.Select(i => new
                {
                    i.BillItemId,
                    i.FoodId,
                    i.Food.FoodName,
                    i.Food.Picture,
                    i.Food.Price,
                    i.Quantity,
                    i.Description,
                    i.SubTotal,
                })
            });
        }

        [HttpPut]
        [Route("/Table/CheckIn")]
        public IActionResult KhachDenNhanBan(int tableId)
        {
            var table = dbc.Tables.Find(tableId);
            if (table == null)
                return NotFound(new { message = "Không tìm thấy bàn!" });

            if (table.StatusId != 2)
                return BadRequest(new { message = "Bàn này chưa được đặt trước hoặc đã có khách sử dụng!" });

            table.StatusId = 3;
            dbc.Tables.Update(table);
            dbc.SaveChanges();

            return Ok(new { message = "Khách đã đến. Bàn đã chuyển sang trạng thái đang sử dụng!" });
        }

        [HttpPost]
        [Route("/Bill/UpsertFood")]
        public IActionResult ThemOrCapNhatMon(int tableId, List<FoodOrderRequestcs> foodOrders)
        {
            var table = dbc.Tables.Find(tableId);

            if (table == null || foodOrders == null || !foodOrders.Any())
                return NotFound(new { message = "Không tìm thấy bàn hoặc món ăn!" });

            // Kiểm tra trạng thái bàn trước khi thêm món
            if (table.StatusId == 2)
                return BadRequest(new { message = "Bàn đã được đặt trước. Chưa thể thêm món khi khách chưa đến.!" });

            var bill = dbc.Bills
                .FirstOrDefault(b => b.TableId == tableId && b.PaidDate == null);

            if (bill == null)
            {
                bill = new Bill
                {
                    TableId = tableId,
                    BranchId = table.BranchId,
                    TotalPrice = 0,
                    Created = DateTime.Now,
                    PaidDate = null
                };

                dbc.Bills.Add(bill);

                // Cập nhật trạng thái bàn thành "đang sử dụng" (nếu chưa)
                table.StatusId = 3;
                dbc.Tables.Update(table);

                dbc.SaveChanges(); // Lưu trước để lấy BillId
            }

            foreach (var i in foodOrders)
            {
                var food = dbc.Foods.Find(i.FoodId);
                if (food == null) continue;

                var existingItem = dbc.BillItems.FirstOrDefault(j => j.BillId == bill.BillId && j.FoodId == i.FoodId);
                BillItem? targetItem;

                if (existingItem == null)
                {
                    targetItem = new BillItem
                    {
                        BillId = bill.BillId,
                        FoodId = i.FoodId,
                        Quantity = i.Quantity,
                        Description = i.Description,
                        SubTotal = food.Price * i.Quantity,
                    };
                    dbc.BillItems.Add(targetItem);
                }
                else
                {
                    existingItem.Quantity = i.Quantity;
                    existingItem.Description = i.Description;
                    existingItem.SubTotal = food.Price * i.Quantity;
                    dbc.BillItems.Update(existingItem);
                    targetItem = existingItem;
                }

                dbc.SaveChanges();
            }

                var updatedItems = dbc.BillItems
                    .Include(i => i.Food)
                    .Where(i => i.BillId == bill.BillId)
                    .Select(i => new
                    {
                        i.BillItemId,
                        i.FoodId,
                        TenMon = i.Food.FoodName,
                        i.Quantity,
                        i.Description,
                        i.SubTotal
                    }).ToList();

            return Ok(new { message = "Cập nhật món ăn thành công!", updatedItems });
        }

        [HttpPost]
        [Route("/Bill/DeleteFoods")]
        public IActionResult XoaNhieuMon([FromBody] List<int> billItemIds)
        {
            var items = dbc.BillItems
                .Include(i => i.Bill)
                .Where(i => billItemIds.Contains(i.BillItemId))
                .ToList();

            if (!items.Any())
                return NotFound(new { message = "Không tìm thấy món nào để xoá!" });

            var bill = items.First().Bill;

            if (bill.PaidDate != null)
                return BadRequest(new { message = "Hóa đơn đã thanh toán, không thể xoá món!" });

            dbc.BillItems.RemoveRange(items);
            dbc.SaveChanges();

            var stillHasItems = dbc.BillItems.Any(i => i.BillId == bill.BillId);
            if (!stillHasItems)
            {
                dbc.Bills.Remove(bill);

                var table = dbc.Tables.Find(bill.TableId);
                if (table != null)
                {
                    table.StatusId = 1;
                    dbc.Tables.Update(table);
                }
                dbc.SaveChanges();

                return Ok(new { message = "Xoá món thành công. Hóa đơn rỗng nên đã xoá và cập nhật trạng thái bàn." });
            }

            return Ok(new { message = "Xoá danh sách món thành công!" });
        }

        [HttpPut]
        [Route("/Bill/Checkout")]
        public IActionResult ThanhToan(int billId, int paymentMethodId)
        {
            var bill = dbc.Bills
                .Include(b => b.BillItems)
                .FirstOrDefault(b => b.BillId == billId);

            if (bill == null)
                return NotFound(new { message = "Không tìm thấy hóa đơn!" });

            if (!bill.BillItems.Any())
                return BadRequest(new { message = "Hóa đơn chưa có món ăn!" });

            bill.TotalPrice = bill.BillItems.Sum(i => i.SubTotal);
            bill.PaidDate = DateTime.Now;
            bill.PaymentMethodId = paymentMethodId;
            dbc.Bills.Update(bill);

            var table = dbc.Tables.FirstOrDefault(t => t.TableId == bill.TableId);
            if (table != null)
            {
                table.StatusId = 1;
                dbc.Tables.Update(table);
            }

            dbc.SaveChanges();

            return Ok(new
            {
                message = "Thanh toán thành công!",
                data = new
                {
                    bill.BillId,
                    bill.TotalPrice,
                    bill.PaidDate,
                    PhuongThucThanhToan = dbc.PaymentMethods.FirstOrDefault(p => p.PaymentMethodId == paymentMethodId)?.PaymentMethodName
                }
            });
        }
    }
}