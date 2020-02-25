using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Goldnote.Data;
using Goldnote.Models;
using System.Web;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Goldnote.Controllers
{

    public class GoldNotesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly MvcGoldnoteContext _context;
        private readonly string imageFolder = "wwwroot/imgs";
        public GoldNotesController(MvcGoldnoteContext context,UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        
[Authorize]
        // GET: GoldNotes
        public async Task<IActionResult> Index()
        {
            var model = await _context.Goldnote.ToListAsync();
            var model_count = model.Count;
            var idNameDic = IdToName(_userManager);
            for(int i = 0; i < model_count; i++)
            {
                model[i].EditerId = idNameDic[model[i].EditerId];
            }

            return View(model);
        }

[Authorize]
        // GET: GoldNotes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var goldNote = await _context.Goldnote
                .FirstOrDefaultAsync(m => m.Id == id);
            if (goldNote == null)
            {
                return NotFound();
            }
            
            var idNameDic = IdToName(_userManager);
            goldNote.EditerId = idNameDic[goldNote.EditerId];
            return View(goldNote);
        }

[Authorize(Roles ="Editor")]
        // GET: GoldNotes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GoldNotes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
[Authorize(Roles="Editor")]
        public async Task<IActionResult> Create([Bind("Id,GoldNoteName,Change,WithDiscount,Destination,OnAccounting,CreditCardMachine,GoldNoteSendingPaper,SpecialOptions,EditerId")] GoldNote goldNote)
        {
            var files = Request.Form.Files;
            if (files.Count() != 0 && files.First().Length!=0)
            {
                var file = files.First();
                if (FileChecker.IsValidFile(file))
                {
                    var filename = DateTime.Now.ToString("yyyyMMddhhmmss") + Path.GetExtension(file.FileName);
                   
                    using (var fs = new FileStream(Path.Join(imageFolder,filename), FileMode.Create, FileAccess.Write))
                    {
                        file.CopyTo(fs);

                    }
                    goldNote.ImageAdress = filename;

                }



             }

            goldNote.EditerId = _userManager.GetUserId(User);
            goldNote.EditDate = DateTime.Today;
            if (ModelState.IsValid)
            {
                _context.Add(goldNote);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(goldNote);
        }

        // GET: GoldNotes/Edit/5
[Authorize(Roles ="Editor,Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var goldNote = await _context.Goldnote.FindAsync(id);
            if (goldNote == null)
            {
                return NotFound();
            }
            var idNameDic = IdToName(_userManager);
            goldNote.EditerId = idNameDic[goldNote.EditerId];
            return View(goldNote);
        }

        // POST: GoldNotes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
    [Authorize(Roles="Editor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GoldNoteName,Change,Destination,OnAccounting,CreditCardMachine,GoldNoteSendingPaper,SpecialOptions,ImageAdress,EditDate,EditerId")] GoldNote goldNote)
        {
            

            if (id != goldNote.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    goldNote.EditerId = _userManager.GetUserId(User);
                    _context.Update(goldNote);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GoldNoteExists(goldNote.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(goldNote);
        }
    [Authorize(Roles = "Administrator,Editor")]
        // GET: GoldNotes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var goldNote = await _context.Goldnote
                .FirstOrDefaultAsync(m => m.Id == id);



            if (goldNote == null)
            {
                return NotFound();
            }
            if (goldNote.EditerId != _userManager.GetUserId(User) && !User.IsInRole("Administrator"))
            {
                return BadRequest();

            }
 
            return View(goldNote);
        }

        // POST: GoldNotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Editor,Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var goldNote = await _context.Goldnote.FindAsync(id);
            if (goldNote.EditerId != _userManager.GetUserId(User) && !User.IsInRole("Administrator"))
            {
                return BadRequest();

            }
            _context.Goldnote.Remove(goldNote);
            if (goldNote.ImageAdress != null)
            {
                System.IO.File.Delete(Path.Join(imageFolder, goldNote.ImageAdress));


            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GoldNoteExists(int id)
        {
            return _context.Goldnote.Any(e => e.Id == id);
        }

        private Dictionary<string,string> IdToName(UserManager<IdentityUser> userManager)
        {
            var dic = new Dictionary<string, string>();

            var users = userManager.Users;
            foreach(var user in users)
            {
                dic.Add(user.Id,user.UserName);
            }


            return dic;
        }
        private Dictionary<string,string> NameToId(UserManager<IdentityUser> userManager)
        {
            var dic = new Dictionary<string, string>();

            var users = userManager.Users;
            foreach(var user in users)
            {
                dic.Add(user.UserName,user.Id);
            }


            return dic;
        }



    }
}
