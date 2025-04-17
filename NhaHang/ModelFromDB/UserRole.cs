using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NhaHang.ModelFromDB;

[Index("RoleName", Name = "UQ__UserRole__B19478611455A72A", IsUnique = true)]
public partial class UserRole
{
    [Key]
    [Column("roleID")]
    public int RoleId { get; set; }

    [Column("roleName")]
    [StringLength(20)]
    public string RoleName { get; set; } = null!;

    [InverseProperty("Role")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
