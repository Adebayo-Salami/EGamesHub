using EGamesData.Models;
using Microsoft.EntityFrameworkCore;

namespace EGamesData
{
    public class EGamesContext : DbContext
    {
        public EGamesContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Bingo> Bingos { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<TransactionHistory> TransactionHistories { get; set; }
        public DbSet<GameHistory> GameHistories { get; set; }
        public DbSet<BrainGameQuestion> BrainGameQuestions { get; set; }
        public DbSet<WordPuzzle> WordPuzzles { get; set; }
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<Withdrawal> Withdrawals { get; set; }
        public DbSet<Promo> Promos { get; set; }
    }
}
