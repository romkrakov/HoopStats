
using Microsoft.EntityFrameworkCore;



namespace HoopStats.Models

{

    public class ApplicationDbContext : DbContext

    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)

            : base(options)

        {

        }

        public DbSet<User> Users { get; set; }


        // Add DbSet properties for your entities here

        // Example: public DbSet<Player> Players { get; set; }

    }

}
