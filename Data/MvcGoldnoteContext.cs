using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Goldnote.Models;



using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Goldnote.Data
{
    public class MvcGoldnoteContext:DbContext
    {
        public MvcGoldnoteContext(DbContextOptions<MvcGoldnoteContext> options) : base(options)
        {

        }

        public DbSet<GoldNote> Goldnote { get; set; }
    }


    public class ImageModelDbContext : DbContext
    {

        public ImageModelDbContext(DbContextOptions<ImageModelDbContext> options) : base(options)
        {

        }
        public DbSet<ImageModel> ImageModels { get; set; }


    }
    public class UserDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }
    }


}
