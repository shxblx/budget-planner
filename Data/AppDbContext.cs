using Microsoft.EntityFrameworkCore;
using BudgetPlanner.Models;

namespace BudgetPlanner.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet <User> Users { get; set; }
        public DbSet <Expense> Expenses { get; set; }
    }
}
