using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NhaHang.ModelFromDB;

public partial class Branch
{
    [Key]
    [Column("branchID")]
    public int BranchId { get; set; }

    [Column("branchName")]
    [StringLength(100)]
    public string BranchName { get; set; } = null!;

    [Column("branchAddr")]
    [StringLength(100)]
    public string BranchAddr { get; set; } = null!;

    [Column("numberPhone")]
    [StringLength(11)]
    public string NumberPhone { get; set; } = null!;

    [Column("image")]
    [StringLength(255)]
    public string? Image { get; set; }

    [InverseProperty("Branch")]
    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    [InverseProperty("Branch")]
    public virtual ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

    [InverseProperty("Branch")]
    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();

    [InverseProperty("Branch")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
