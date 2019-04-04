namespace Dashboard.DataBase
{
    using BafghAutomation.Engine.Models;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public class AppDataContext : DbContext
    {

        public AppDataContext()
            : base("name=AppDataContext")
        {
            Database.SetInitializer<AppDataContext>(null);
        }

        public DbSet<Pack> Packs { get; set; }
        public DbSet<SentItem> SentItems { get; set; }
        public DbSet<Good> Goods { get; set; }
    }
}
