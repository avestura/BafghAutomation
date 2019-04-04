using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.DataBase
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public partial class AppDataContext : DbContext
    {

        public AppDataContext()
            : base("name=AppDataContext")
        {
        }

        public DbSet<Packs> Packs { get; set; }
        public DbSet<SentItems> SentItems { get; set; }
        public DbSet<ItemCodes> ItemCodes { get; set; }
    }
}
