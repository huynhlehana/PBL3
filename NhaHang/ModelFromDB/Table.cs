using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NhaHang.ModelFromDB;

public partial class Table
{
    [Key]
    [Column("tableID")]
    public int TableId { get; set; }

    [Column("tableNumber")]
    public int TableNumber { get; set; }

    [Column("capacity")]
    public int Capacity { get; set; }

    [Column("statusID")]
    public int StatusId { get; set; }

    [Column("branchID")]
    public int BranchId { get; set; }

    [InverseProperty("Table")]
    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    [ForeignKey("BranchId")]
    [InverseProperty("Tables")]
    public virtual Branch Branch { get; set; } = null!;

    [InverseProperty("Table")]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    [ForeignKey("StatusId")]
    [InverseProperty("Tables")]
    public virtual Status Status { get; set; } = null!;
}
