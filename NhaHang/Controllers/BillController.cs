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
        [Route("/Bill/GetOrCreateByTable")]
        public IActionResult LayHoacTaoHoaDonTheoBan(int tableId, int branchId)
        {
            var table = dbc.Tables.Find(tableId);

            if (table == null)
                return NotFound(new { message = "Không tìm thấy bàn!" });

            // Nếu bàn đang được đặt trước
            if (table.StatusId == 2)
            {
                return BadRequest(new { message = "Bàn đã được đặt trước. Chưa thể tạo hóa đơn khi khách chưa đến." });
            }

            // Nếu bàn đang sử dụng
            if (table.StatusId == 3)
            {
                var existingBill = dbc.Bills
                    .Include(b => b.BillItems)
                    .ThenInclude(i => i.Food)
                    .FirstOrDefault(b => b.TableId == tableId && b.PaidDate == null);

                if (existingBill != null)
                {
                    return Ok(new
                    {
                        message = "Đã có hóa đơn đang mở cho bàn này",
                        billId = existingBill.BillId,
                        danhSachMon = existingBill.BillItems.Select(i => new
                        {
                            i.BillItemId,
                            i.FoodId,
                            TenMon = i.Food.FoodName,
                            i.Quantity,
                            i.Description,
                            i.SubTotal
                        })
                    });
                }
                else
                {
                    // Nếu chưa có hóa đơn, thì tạo hóa đơn mới tại đây
                    var newBill = new Bill
                    {
                        TableId = tableId,
                        BranchId = branchId,
                        TotalPrice = 0,
                        PaidDate = null,
                        PaymentMethodId = null
                    };

                    dbc.Bills.Add(newBill);
                    dbc.SaveChanges();

                    return Ok(new
                    {
                        message = "Tạo hóa đơn mới thành công cho bàn đặt trước",
                        billId = newBill.BillId,
                        danhSachMon = new List<object>()
                    });
                }
            }

            // Nếu bàn đang trống → tạo hóa đơn mới
            if (table.StatusId == 1)
            {
                var bill = new Bill
                {
                    TableId = tableId,
                    BranchId = branchId,
                    TotalPrice = 0,
                    PaidDate = null,
                    PaymentMethodId = null
                };

                dbc.Bills.Add(bill);

                table.StatusId = 3;
                dbc.Tables.Update(table);

                dbc.SaveChanges();

                return Ok(new
                {
                    message = "Tạo hóa đơn mới thành công",
                    billId = bill.BillId,
                    danhSachMon = new List<object>()
                });
            }

            return BadRequest(new { message = "Trạng thái bàn không hợp lệ!" });
        }

        [HttpDelete]
        [Route("/Bill/DeleteFood")]
        public IActionResult XoaMonKhoiHoaDon(int billItemId)
        {
            var billItem = dbc.BillItems
                .Include(i => i.Bill)
                .FirstOrDefault(i => i.BillItemId == billItemId);

            if (billItem == null)
                return NotFound(new { message = "Không tìm thấy món trong hóa đơn!" });

            // Kiểm tra hóa đơn đã thanh toán chưa
            if (billItem.Bill.PaidDate != null)
                return BadRequest(new { message = "Hóa đơn đã thanh toán. Không thể xoá món!" });

            dbc.BillItems.Remove(billItem);
            dbc.SaveChanges();

            return Ok(new { message = "Xoá món khỏi hóa đơn thành công!", deletedItemId = billItemId });
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
        public IActionResult ThemOrCapNhatMon(int billId, int foodId, int soLuong, string? ghiChu)
        {
            var bill = dbc.Bills.Find(billId);
            var food = dbc.Foods.Find(foodId);

            if (bill == null || food == null)
            {
                return NotFound(new { message = "Không tìm thấy hóa đơn hoặc món ăn!" });
            }

            BillItem? targetItem;
            var existingItem = dbc.BillItems.FirstOrDefault(i => i.BillId == billId && i.FoodId == foodId);

            if (existingItem == null)
            {
                var newItem = new BillItem
                {
                    BillId = billId,
                    FoodId = foodId,
                    Quantity = soLuong,
                    Description = ghiChu,
                    SubTotal = food.Price * soLuong
                };

                dbc.BillItems.Add(newItem);
                targetItem = newItem;
            }
            else
            {
                if (existingItem.Quantity != soLuong || existingItem.Description != ghiChu)
                {
                    existingItem.Quantity = soLuong;
                    existingItem.Description = ghiChu;
                    existingItem.SubTotal = food.Price * soLuong;

                    dbc.BillItems.Update(existingItem);
                }
                targetItem = existingItem;
            }

            dbc.SaveChanges();
            var monMoi = dbc.BillItems
                .Include(i => i.Food)
                .Where(i => i.BillItemId == targetItem!.BillItemId)
                .Select(i => new
                {
                    i.BillItemId,
                    i.FoodId,
                    TenMon = i.Food.FoodName,
                    i.Quantity,
                    i.Description,
                    i.SubTotal
                })
                .FirstOrDefault();

            return Ok(new { message = "Cập nhật món ăn thành công!", monMoi });
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

            decimal tongTien = bill.BillItems.Sum(i => i.SubTotal);

            bill.TotalPrice = tongTien;
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
