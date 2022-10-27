﻿using DataSettingsSample.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#nullable disable

namespace DataSettingsSample.Data
{
    public class UserGeometryDbContext : DbCustomContext
    {
        public DbSet<UserGeometry> UserGeometries { get; set; }

        public UserGeometryDbContext(string connectionString, string tableName/*, DbContextOptions<UserGeometryDbContext> options*/) : base(connectionString, tableName/*, options*/)
        {

        }
        public override IQueryable<object> GetTable() => UserGeometries.Cast<object>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserGeometry>().ToTable(TableName);

            // UserGeometries
            modelBuilder.Entity<UserGeometry>(UserGeometriesConfigure);
        }

        protected static void UserGeometriesConfigure(EntityTypeBuilder<UserGeometry> builder)
        {
            builder.Property(b => b.Value).IsRequired();
            builder.HasKey(b => b.Value);
        }

   //     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
   // => optionsBuilder.UseNpgsql(@"Host=localhost;Username=postgres;Password=user;Database=DataSettingsSampleDatabase2");

        public override async Task<IList<object>> ToListAsync() => await UserGeometries.Cast<object>().ToListAsync().ConfigureAwait(false);
    }
}
