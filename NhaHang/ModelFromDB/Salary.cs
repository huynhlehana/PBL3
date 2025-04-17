using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NhaHang.ModelFromDB;

[Table("Salary")]
public partial class Salary
{
    [Key]
    [Column("salaryID")]
    public int SalaryId { get; set; }

    [Column("userID")]
    public int UserId { get; set; }

    [Column("money", TypeName = "decimal(8, 0)")]
    public decimal Money { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Salaries")]
    public virtual User User { get; set; } = null!;
}
