using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Goldnote.Models;



using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Goldnote.Data
{
    public class MvcGoldnoteContext:DbContext
    {
        public MvcGoldnoteContext(DbContextOptions<MvcGoldnoteContext> options) : base(options)
        {

        }

        public DbSet<GoldNote> Goldnote { get; set; }
    }



    public class UserDbContext : IdentityDbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }
    }


}
