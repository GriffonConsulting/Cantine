using EntityFramework.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Client> Client { get; set; }
        public DbSet<ClientAccount> ClientAccount { get; set; }
        public DbSet<ClientAccountTransactionHistory> ClientAccountTransactionHistory { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderContent> OrderContent { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Role> Role { get; set; }
    }
}