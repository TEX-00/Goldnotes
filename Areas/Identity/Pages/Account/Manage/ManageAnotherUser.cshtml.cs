using System;
using Goldnote.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
namespace Goldnote.Areas.Identity.Pages.Account.Manage
{
    
    [Authorize(Roles = "Administrator")]
    public class ManageAnotherUserModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        
        public List<User> users { get; set; }
        public ManageAnotherUserModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            
        }

        public async Task OnGet()
        {
            var sourceUsers=_userManager.Users.ToList();
            var you=   await _userManager.GetUserAsync(User);

            users = new List<User>();
            sourceUsers.Remove(you);
            foreach(var user in sourceUsers)
            {
                User newUser = new User() { id = user.Id, name = user.UserName };
                newUser.isAdmin = await _userManager.IsInRoleAsync(user, "Administrator");
                newUser.isEditor= await _userManager.IsInRoleAsync(user, "Editor");
                users.Add(newUser);

            }
        }
    }

}
