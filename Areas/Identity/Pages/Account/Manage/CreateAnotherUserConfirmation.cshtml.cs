using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Goldnote.Areas.Identity.Pages.Account.Manage
{
    public class CreateAnotherUserConfirmationModel:PageModel

    {
        private readonly UserManager<IdentityUser> _userManager;

        public CreateAnotherUserConfirmationModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public string Name { get; set; }

        public string temppass { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {

        /*    if (Name == null | temppass==null)
            {
                return NotFound($"ThisPageMustRedirected");
            }
            */
            return Page();
        }
    }
}
