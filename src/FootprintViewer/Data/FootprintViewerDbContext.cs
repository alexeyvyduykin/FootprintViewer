﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace FootprintViewer.Data
{
    public class FootprintViewerDbContext : DbContext
    {
        public DbSet<Satellite> Satellites { get; set; }
        public DbSet<GroundTarget> GroundTargets { get; set; }
        public DbSet<Footprint> Footprints { get; set; }
        public DbSet<UserGeometry> UserGeometries { get; set; }

        public FootprintViewerDbContext(DbContextOptions<FootprintViewerDbContext> options) : base(options)
        {       
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    var options = optionsBuilder.Options;

        //    optionsBuilder.UseNpgsql("Host=localhost;Database=my_db2;Username=postgres;Password=user",
        //        options => options.SetPostgresVersion(new Version(9, 6)));
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");

            // Satellites
            modelBuilder.Entity<Satellite>(SatelliteConfigure);

            // GroudnTargets
            modelBuilder.Entity<GroundTarget>(GroundTargetConfigure);

            // Footprints
            modelBuilder.Entity<Footprint>(FootprintConfigure);            
            
            // UserGeometries
            modelBuilder.Entity<UserGeometry>(UserGeometriesConfigure);
        }

        protected void SatelliteConfigure(EntityTypeBuilder<Satellite> builder)
        {
            builder.Property(b => b.Name).IsRequired();
            builder.HasKey(b => b.Name);         
        }

        protected void GroundTargetConfigure(EntityTypeBuilder<GroundTarget> builder)
        {
            builder.Property(b => b.Name).IsRequired();
            builder.HasKey(b => b.Name);
            builder.Property(e => e.Type).HasConversion(
                v => v.ToString(),
                v => (GroundTargetType)Enum.Parse(typeof(GroundTargetType), v));
        }

        protected void FootprintConfigure(EntityTypeBuilder<Footprint> builder)
        {
            builder.Property(b => b.Name).IsRequired();
            builder.HasKey(b => b.Name);
            builder.Property(e => e.Direction).HasConversion(
                v => v.ToString(),
                v => (SatelliteStripDirection)Enum.Parse(typeof(SatelliteStripDirection), v));
        }

        protected void UserGeometriesConfigure(EntityTypeBuilder<UserGeometry> builder)
        {
            builder.Property(b => b.Name).IsRequired();
            builder.HasKey(b => b.Name);
            builder.Property(e => e.Type).HasConversion(
                v => v.ToString(),
                v => (UserGeometryType)Enum.Parse(typeof(UserGeometryType), v));
        }
    }
}
