using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TodoWebApiProjectWithUserAuthentication.Models.Entities;

namespace TodoWebApiProjectWithUserAuthentication.Data
{
    public class ListDbContext : IdentityUserContext<IdentityUser>
    {
        public DbSet<List> List { get; set; }
        public DbSet<ListItem> ListItem { get; set; }
        public ListDbContext(DbContextOptions<ListDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);          
        }
    }
}

