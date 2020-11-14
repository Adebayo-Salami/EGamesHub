using Microsoft.EntityFrameworkCore;

namespace EGameData
{
    public class EGamesContext : DbContext
    {
        public EGamesContext(DbContextOptions options) : base(options) { }


    }
}
