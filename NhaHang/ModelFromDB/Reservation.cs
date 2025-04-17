using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NhaHang.ModelFromDB;

public partial class Reservation
{
    [Key]
    [Column("reservationID")]
    public int ReservationId { get; set; }

    [Column("tableID")]
    public int TableId { get; set; }

    [Column("userID")]
    public int UserId { get; set; }

    [Column("bookingTime", TypeName = "datetime")]
    public DateTime? BookingTime { get; set; }

    [Column("isArrived")]
    public bool? IsArrived { get; set; }

    [ForeignKey("TableId")]
    [InverseProperty("Reservations")]
    public virtual Table Table { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Reservations")]
    public virtual User User { get; set; } = null!;
}
