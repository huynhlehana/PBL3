using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace NhaHang.ModelFromDB;

public partial class quanlynhahang : DbContext
{
    public quanlynhahang()
    {
    }

    public quanlynhahang(DbContextOptions<quanlynhahang> options)
        : base(options)
    {
    }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<BillItem> BillItems { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Food> Foods { get; set; }

    public virtual DbSet<Gender> Genders { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Salary> Salaries { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }
}
