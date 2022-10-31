using DataSettingsSample.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#nullable disable

namespace DataSettingsSample.Data
{
    public class SatelliteDbContext : DbCustomContext
    {
        public DbSet<Satellite> Satellites { get; set; }

        public SatelliteDbContext(string connectionString, string tableName) : base(connectionString, tableName)
        {

        }

        public override IQueryable<object> GetTable() => Satellites.Cast<object>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Satellite>().ToTable(TableName);

            // Satellites
            modelBuilder.Entity<Satellite>(SatelliteConfigure);
        }

        protected static void SatelliteConfigure(EntityTypeBuilder<Satellite> builder)
        {
            builder.Property(b => b.ValueInt).IsRequired();
            builder.HasKey(b => b.ValueInt);
        }

        public override async Task<IList<object>> ToListAsync() => await Satellites.Cast<object>().ToListAsync().ConfigureAwait(false);
    }
}
