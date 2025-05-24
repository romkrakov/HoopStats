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
        public DbSet<GameStats> GameStats { get; set; }


        // Add DbSet properties for your entities here

        // Example: public DbSet<Player> Players { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure indexes for GameStats table
            modelBuilder.Entity<GameStats>()
                .HasIndex(g => g.GameDate);
                
            modelBuilder.Entity<GameStats>()
                .HasIndex(g => new { g.Team, g.Opponent });
                
            modelBuilder.Entity<GameStats>()
                .HasIndex(g => g.Player);
        }
    }

}
