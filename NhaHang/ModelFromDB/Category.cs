using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NhaHang.ModelFromDB;

public partial class Category
{
    [Key]
    [Column("categoryID")]
    public int CategoryId { get; set; }

    [Column("categoryName")]
    [StringLength(20)]
    public string CategoryName { get; set; } = null!;

    [InverseProperty("Category")]
    public virtual ICollection<Food> Foods { get; set; } = new List<Food>();
}
