using Microsoft.EntityFrameworkCore;
using PortalPegawai.Models;

namespace PortalPegawai.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Transaksi> transaksi { get; set; }
}
