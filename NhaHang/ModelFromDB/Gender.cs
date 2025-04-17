using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NhaHang.ModelFromDB;

[Table("Gender")]
[Index("GenderName", Name = "UQ__Gender__14B63E73C82A703A", IsUnique = true)]
public partial class Gender
{
    [Key]
    [Column("genderID")]
    public int GenderId { get; set; }

    [Column("genderName")]
    [StringLength(10)]
    public string GenderName { get; set; } = null!;

    [InverseProperty("Gender")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
