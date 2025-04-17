using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NhaHang.ModelFromDB;

public partial class PaymentMethod
{
    [Key]
    [Column("paymentMethodID")]
    public int PaymentMethodId { get; set; }

    [Column("paymentMethodName")]
    [StringLength(15)]
    public string PaymentMethodName { get; set; } = null!;

    [InverseProperty("PaymentMethod")]
    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
}
