using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NhaHang.ModelFromDB;

public partial class BillItem
{
    [Key]
    [Column("billItemID")]
    public int BillItemId { get; set; }

    [Column("billID")]
    public int BillId { get; set; }

    [Column("foodID")]
    public int FoodId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("description")]
    [StringLength(100)]
    public string? Description { get; set; }

    [Column("subTotal", TypeName = "decimal(7, 0)")]
    public decimal SubTotal { get; set; }

    [ForeignKey("BillId")]
    [InverseProperty("BillItems")]
    public virtual Bill Bill { get; set; } = null!;

    [ForeignKey("FoodId")]
    [InverseProperty("BillItems")]
    public virtual Food Food { get; set; } = null!;
}
