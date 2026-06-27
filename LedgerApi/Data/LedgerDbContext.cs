using LedgerApi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LedgerApi.Data
{
    public class LedgerDbContext : DbContext
    {
        public LedgerDbContext(DbContextOptions<LedgerDbContext> options) : base(options) { }

        public DbSet<LedgerEvent> LedgerEvents { get; set; }
        public DbSet<AccountBalance> AccountBalances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // LedgerEvent table config
            modelBuilder.Entity<LedgerEvent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.Payload).HasColumnType("json");
                entity.HasIndex(e => e.AccountId);
                entity.HasIndex(e => new { e.AccountId, e.Version }).IsUnique();
            });

            // AccountBalance table config
            modelBuilder.Entity<AccountBalance>(entity =>
            {
                entity.HasKey(e => e.AccountId);
                entity.Property(e => e.Balance).HasPrecision(18, 2);
            });
        }
    }
}
