using Microsoft.EntityFrameworkCore;
using BecamexIDC.Authentication.Models.Entities.BCMAppModels;

namespace BecamexIDC.Authentication.Data
{
    public class BCMAppDbContext : DbContext
    {
        public BCMAppDbContext(DbContextOptions<BCMAppDbContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ViewUsers>(vu =>
            {
                vu.HasNoKey();
                vu.ToView("view_users");
            });
            modelBuilder.Entity<ViewDepartments>(vd =>
            {
                vd.HasNoKey();
                vd.ToView("view_departments");
            });
        }
        public DbSet<ViewUsers> ViewUsers { get; set; }
        public DbSet<ViewDepartments> ViewDepartments { get; set; }
    }
}