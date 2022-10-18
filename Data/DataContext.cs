using Microsoft.EntityFrameworkCore;

#nullable disable

namespace dotnet_rpg.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
           
        }
        // cand vrei sa vezi o reprezentare a unui model in db folosesti DbSet
         public DbSet<Character> Characters { get; set; }
         public DbSet<User> Users { get; set; }

    }
}