using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NhaHang.ModelFromDB;

public partial class Ingredient
{
    [Key]
    [Column("ingredientID")]
    public int IngredientId { get; set; }

    [Column("ingredientName")]
    [StringLength(20)]
    public string IngredientName { get; set; } = null!;

    [Column("unitID")]
    public int UnitId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("branchId")]
    public int BranchId { get; set; }

    [ForeignKey("BranchId")]
    [InverseProperty("Ingredients")]
    public virtual Branch Branch { get; set; } = null!;

    [ForeignKey("UnitId")]
    [InverseProperty("Ingredients")]
    public virtual Unit Unit { get; set; } = null!;
}
