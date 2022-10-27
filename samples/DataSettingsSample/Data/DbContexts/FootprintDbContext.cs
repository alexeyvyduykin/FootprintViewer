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
      //  private readonly string _connectionString;

        public DbSet<Footprint> Footprints { get; set; }

        public FootprintDbContext(string connectionString, string tableName/*, DbContextOptions<FootprintDbContext> options*/) : base(connectionString, tableName/*, options*/)
        {
         //   _connectionString = connectionString;
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
               
     //   protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            //=> optionsBuilder.UseNpgsql(@"Host=localhost;Username=postgres;Password=user;Database=DataSettingsSampleDatabase2");
    //        => optionsBuilder.UseNpgsql(_connectionString);

        public override async Task<IList<object>> ToListAsync() => await Footprints.Cast<object>().ToListAsync().ConfigureAwait(false);
    }
}
