using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NhaHang.ModelFromDB;

[Table("Status")]
public partial class Status
{
    [Key]
    [Column("statusID")]
    public int StatusId { get; set; }

    [Column("statusName")]
    [StringLength(20)]
    public string StatusName { get; set; } = null!;

    [InverseProperty("Status")]
    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();
}
