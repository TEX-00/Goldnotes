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
    public class AnotherUserDetailPageModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AnotherUserDetailPageModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;

        }
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Id")]
            public string Id { get; set; }
            
             [Required]
            [Display(Name = "Name")]
            public string Name { get; set; }
        
              [Required]
            [Display(Name = "ä«óùé“")]
            public bool isAdmin { get; set; }
               [Required]
            [Display(Name = "ï“èWé“")]
            public bool isEditor { get; set; }
     
        }
        public bool isAdmin;
        public bool isEditor;
        public IdentityUser targetUser;
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound("Id is Required");
            }
            targetUser = await _userManager.FindByIdAsync(id);

            if(targetUser == null)
            {
                return NotFound("No User Has Id ="+id);
            }

            isAdmin = await _userManager.IsInRoleAsync(targetUser, "Administrator");

            isEditor = await _userManager.IsInRoleAsync(targetUser, "Editor");

            return Page();

        }

        public async Task<IActionResult> OnPostChangeAnotherUserAsync()
        {
             targetUser=await _userManager.FindByIdAsync(Input.Id);
            if(targetUser==null | Input.Name != targetUser.UserName)
            {
                return NotFound();
            }

            if (Input.isAdmin && !await _userManager.IsInRoleAsync(targetUser, "Administrator"))
            {
                await _userManager.AddToRoleAsync(targetUser, "Administrator");
            }
            else if (!Input.isAdmin && await _userManager.IsInRoleAsync(targetUser, "Administrator"))
            {
                await _userManager.RemoveFromRoleAsync(targetUser, "Administrator"); 
            
            }

            if (Input.isEditor && !await _userManager.IsInRoleAsync(targetUser, "Editor"))
            {
                await _userManager.AddToRoleAsync(targetUser, "Editor");
            }
            else if (!Input.isEditor && await _userManager.IsInRoleAsync(targetUser, "Editor"))
            {
                await _userManager.RemoveFromRoleAsync(targetUser, "Editor"); 
            
            }


            return Redirect("/Identity/Account/Manage/ManageAnotherUser");

        }


        public async Task<IActionResult> OnPostDeleteAnotherUserAsync()
        {
            targetUser = await _userManager.FindByIdAsync(Input.Id);
            if (targetUser == null | Input.Name != targetUser.UserName)
            {
                return NotFound();
            }
            if (await _userManager.IsInRoleAsync(targetUser, "Administrator"))
            {
                return BadRequest("Administrator user cannot be deleted");
            }
            await _userManager.DeleteAsync(targetUser);

            return Redirect("/Identity/Account/Manage/ManageAnotherUser");

        }


    }
}
