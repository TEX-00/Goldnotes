using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
namespace Goldnote.Models
{
    public class User:IdentityUser
    {

        public DateTime Created { get; set; }
    }
}
