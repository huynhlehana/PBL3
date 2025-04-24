using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using NhaHang.ModelFromDB;
using Microsoft.EntityFrameworkCore;

namespace NhaHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public IActionResult ThemOrCapNhatMon(int tableId, [FromBody] List<BillItem> foodOrders)
        {
            var table = dbc.Tables.Find(tableId);

            if (table == null)
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
                    PaidDate = null
                };

                dbc.Bills.Add(bill);

                // Cập nhật trạng thái bàn thành "đang sử dụng" (nếu chưa)
                table.StatusId = 3;
                dbc.Tables.Update(table);

                dbc.SaveChanges(); // Lưu trước để lấy BillId
            }

            var danhSachMonMoi = new List<object>();

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

                var updatedItems = dbc.BillItems
                    .Include(i => i.Food)
                    .Where(i => i.BillItemId == targetItem.BillItemId)
                    .Select(i => new
                    {
                        i.BillItemId,
                        i.FoodId,
                        TenMon = i.Food.FoodName,
                        i.Quantity,
                        i.Description,
                        i.SubTotal
                    }).FirstOrDefault();

                if (updatedItems != null)
                    danhSachMonMoi.Add(updatedItems);
            }

            return Ok(new { message = "Cập nhật món ăn thành công!",danhSachMonMoi});
        }

        [HttpDelete]
        [Route("/Bill/DeleteFoods")]
        public IActionResult XoaNhieuMonKhoiHoaDon([FromBody] List<BillItem> itemsToDelete)
        {
            if (itemsToDelete == null || !itemsToDelete.Any())
                return BadRequest(new { message = "Danh sách món cần xoá không hợp lệ!" });

            // Giả sử tất cả BillItem trong 1 bill
            var firstItem = itemsToDelete.First();
            var firstBillItem = dbc.BillItems
                .Include(i => i.Bill)
                .FirstOrDefault(i => i.BillItemId == firstItem.BillItemId);

            if (firstBillItem == null)
                return NotFound(new { message = "Không tìm thấy hóa đơn hoặc món ăn!" });

            var bill = firstBillItem.Bill;

            if (bill.PaidDate != null)
                return BadRequest(new { message = "Hóa đơn đã thanh toán. Không thể xoá món!" });

            foreach (var item in itemsToDelete)
            {
                var billItem = dbc.BillItems.FirstOrDefault(i => i.BillItemId == item.BillItemId);
                if (billItem != null && billItem.BillId == bill.BillId)
                {
                    dbc.BillItems.Remove(billItem);
                }
            }

            dbc.SaveChanges();

            var remainingItems = dbc.BillItems.Any(i => i.BillId == bill.BillId);
            if (!remainingItems)
            {
                dbc.Bills.Remove(bill);
                var table = dbc.Tables.Find(bill.TableId);
                if (table != null)
                {
                    table.StatusId = 1;
                    dbc.Tables.Update(table);
                }

                dbc.SaveChanges();
                return Ok(new { message = "Xoá món thành công. Hóa đơn không còn món nào nên đã xoá và cập nhật trạng thái bàn về trống." });
            }

            return Ok(new { message = "Xoá các món khỏi hóa đơn thành công!" });
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