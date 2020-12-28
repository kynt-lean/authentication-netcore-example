
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BecamexIDC.Authentication.Domain;
using Microsoft.AspNetCore.Identity;
using BecamexIDC.Authentication.Models;
using BecamexIDC.Authentication.Models.Entities;

namespace BecamexIDC.Authentication.Data
{
    public class AuthDbContext : IdentityDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permissions>(entity =>
            {
                entity.Property(e => e.RoleId).HasMaxLength(450);
            });
            modelBuilder.Entity<UserFactory>().HasKey(x => new
            {
                x.FactoryId,
                x.UserId
            });
          
            base.OnModelCreating(modelBuilder);          
        }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Policies> Policies { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<EOfficeUserInfo> EOfficeUserInfo { get; set; }
        public DbSet<UserFactory> UserFactory { get; set; }
    }
}