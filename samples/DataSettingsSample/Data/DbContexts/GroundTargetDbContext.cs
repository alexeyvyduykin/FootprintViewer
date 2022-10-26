using FDataSettingsSample.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#nullable disable

namespace DataSettingsSample.Data
{
    public class GroundTargetDbContext : DbCustomContext
    {
        public DbSet<GroundTarget> GroundTargets { get; set; }

        public GroundTargetDbContext(string tableName/*, DbContextOptions<GroundTargetDbContext> options*/) : base(tableName/*, options*/)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GroundTarget>().ToTable(TableName);

            // GroudnTargets
            modelBuilder.Entity<GroundTarget>(GroundTargetConfigure);
        }

        protected static void GroundTargetConfigure(EntityTypeBuilder<GroundTarget> builder)
        {
            builder.Property(b => b.Value).IsRequired();
            builder.HasKey(b => b.Value);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseNpgsql(@"Host=localhost;Username=postgres;Password=user;Database=DataSettingsSampleDatabase2");

        public override async Task<IList<object>> ToListAsync() => await GroundTargets.Cast<object>().ToListAsync().ConfigureAwait(false);
    }
}
