using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ZCore.Model;

namespace EntityFrameworkCore
{
    public class ZDomainContext : DbContext
    {
        public ZDomainContext()
        {

        }

        public ZDomainContext(DbContextOptions<ZDomainContext> options)
           : base(options)
        {
        }

        public static string ConnnectString { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(ConnnectString);
            }
        }
        protected virtual DbSet<MemberInfo> MemberInfo { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MemberInfo>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("member_info");

                entity.HasIndex(e => e.AccountId)
                .IsUnique();

                entity.Property(e => e.AccountId)
                .HasColumnType("varchar(50)");

                entity.Property(e => e.MemberName)
                .HasColumnType("varchar(50)");

            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
