using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NhaHang.ModelFromDB;

[Table("Food")]
public partial class Food
{
    [Key]
    [Column("foodID")]
    public int FoodId { get; set; }

    [Column("foodName")]
    [StringLength(30)]
    public string FoodName { get; set; } = null!;

    [Column("categoryID")]
    public int CategoryId { get; set; }

    [Column("price", TypeName = "decimal(6, 0)")]
    public decimal Price { get; set; }

    [Column("picture")]
    [StringLength(255)]
    public string? Picture { get; set; }

    [InverseProperty("Food")]
    public virtual ICollection<BillItem> BillItems { get; set; } = new List<BillItem>();

    [ForeignKey("CategoryId")]
    [InverseProperty("Foods")]
    public virtual Category Category { get; set; } = null!;
}
