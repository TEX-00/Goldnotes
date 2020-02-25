using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
namespace Goldnote.Models
{
    public class User
    {
       public string id { get; set; }
        public string name { get; set; }
        public bool isAdmin { get; set; }
        public bool isEditor { get; set; }

    }
}
