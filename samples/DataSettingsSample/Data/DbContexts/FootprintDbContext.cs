using DataSettingsSample.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#nullable disable

namespace DataSettingsSample.Data
{
    public class FootprintDbContext : DbCustomContext
    {
        public DbSet<Footprint> Footprints { get; set; }

        public FootprintDbContext(string connectionString, string tableName) : base(connectionString, tableName)
        {
        }

        public override IQueryable<object> GetTable() => Footprints.Cast<object>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Footprint>().ToTable(TableName);

            // Footprints
            modelBuilder.Entity<Footprint>(FootprintConfigure);
        }

        protected static void FootprintConfigure(EntityTypeBuilder<Footprint> builder)
        {
            builder.Property(b => b.Value).IsRequired();
            builder.HasKey(b => b.Value);
        }

        public override async Task<IList<object>> ToListAsync() => await Footprints.Cast<object>().ToListAsync().ConfigureAwait(false);
    }
}
