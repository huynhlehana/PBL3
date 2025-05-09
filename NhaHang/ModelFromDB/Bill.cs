using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NhaHang.ModelFromDB;

[Table("Bill")]
public partial class Bill
{
    [Key]
    [Column("billID")]
    public int BillId { get; set; }

    [Column("tableID")]
    public int TableId { get; set; }

    [Column("totalPrice", TypeName = "decimal(8, 0)")]
    public decimal TotalPrice { get; set; }

    [Column("paymentMethodID")]
    public int? PaymentMethodId { get; set; }

    [Column("created", TypeName = "datetime")]
    public DateTime? Created { get; set; }

    [Column("paidDate", TypeName = "datetime")]
    public DateTime? PaidDate { get; set; }

    [Column("branchId")]
    public int BranchId { get; set; }

    [InverseProperty("Bill")]
    public virtual ICollection<BillItem> BillItems { get; set; } = new List<BillItem>();

    [ForeignKey("BranchId")]
    [InverseProperty("Bills")]
    public virtual Branch Branch { get; set; } = null!;

    [ForeignKey("PaymentMethodId")]
    [InverseProperty("Bills")]
    public virtual PaymentMethod? PaymentMethod { get; set; }

    [ForeignKey("TableId")]
    [InverseProperty("Bills")]
    public virtual Table Table { get; set; } = null!;
}
