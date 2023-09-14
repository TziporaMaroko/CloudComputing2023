using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZeldaWebsite.Models;

namespace ZeldaWebsite.Data
{
    public class ZeldaWebsiteContext : DbContext
    {
        public ZeldaWebsiteContext()
        {
        }

        public ZeldaWebsiteContext(DbContextOptions<ZeldaWebsiteContext> options)
            : base(options)
        {
        }

        public DbSet<ZeldaWebsite.Models.Flavour> Flavour { get; set; } = default!;
        public DbSet<ZeldaWebsite.Models.CartItem> ShoppingCartItems { get; set; }
    }
}
