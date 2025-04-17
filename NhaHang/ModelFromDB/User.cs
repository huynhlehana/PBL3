using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NhaHang.ModelFromDB;

[Index("PhoneNumber", Name = "UQ__Users__4849DA01672100C7", IsUnique = true)]
[Index("UserName", Name = "UQ__Users__66DCF95CF5FEA206", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("userID")]
    public int UserId { get; set; }

    [Column("userName")]
    [StringLength(50)]
    public string UserName { get; set; } = null!;

    [Column("password")]
    [StringLength(50)]
    public string Password { get; set; } = null!;

    [Column("firstName")]
    [StringLength(10)]
    public string FirstName { get; set; } = null!;

    [Column("lastName")]
    [StringLength(10)]
    public string LastName { get; set; } = null!;

    [Column("phoneNumber")]
    [StringLength(11)]
    public string PhoneNumber { get; set; } = null!;

    [Column("birthDay")]
    public DateOnly BirthDay { get; set; }

    [Column("genderID")]
    public int GenderId { get; set; }

    [Column("roleID")]
    public int RoleId { get; set; }

    [Column("picture")]
    [StringLength(255)]
    public string? Picture { get; set; }

    [Column("createAt", TypeName = "datetime")]
    public DateTime? CreateAt { get; set; }

    [Column("branchID")]
    public int BranchId { get; set; }

    [ForeignKey("BranchId")]
    [InverseProperty("Users")]
    public virtual Branch Branch { get; set; } = null!;

    [ForeignKey("GenderId")]
    [InverseProperty("Users")]
    public virtual Gender Gender { get; set; } = null!;

    [InverseProperty("User")]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual UserRole Role { get; set; } = null!;

    [InverseProperty("User")]
    public virtual ICollection<Salary> Salaries { get; set; } = new List<Salary>();
}
