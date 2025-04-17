using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NhaHang.ModelFromDB;

[Table("Unit")]
public partial class Unit
{
    [Key]
    [Column("unitID")]
    public int UnitId { get; set; }

    [Column("unitName")]
    [StringLength(20)]
    public string UnitName { get; set; } = null!;

    [InverseProperty("Unit")]
    public virtual ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
}
