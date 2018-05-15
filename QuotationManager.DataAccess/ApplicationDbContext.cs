using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using QuotationManager.Models;

namespace QuotationManager.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Quota> Quotas { get; set; }
        public virtual DbSet<Contribution> Contributions { get; set; }
        public virtual DbSet<AdditionalContribution> AdditionalContributions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Quota>()
                .HasMany(q => q.AdditionalContributions)
                .WithOne(a => a.Quota)
                .HasForeignKey(a => a.QuotaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
