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
        private readonly MvcGoldnoteContext _context;
        private readonly string imageFolder = "wwwroot/imgs";
        public GoldNotesController(MvcGoldnoteContext context)
        {
            _context = context;
        }

        
[Authorize]
        // GET: GoldNotes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Goldnote.ToListAsync());
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

            return View(goldNote);
        }

[Authorize]
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
[Authorize]
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

            goldNote.EditerId = User.Identity.Name;
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
[Authorize]
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
            return View(goldNote);
        }

        // POST: GoldNotes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
[Authorize]
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

            return View(goldNote);
        }

        // POST: GoldNotes/Delete/5
        [HttpPost, ActionName("Delete")]
[Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var goldNote = await _context.Goldnote.FindAsync(id);

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
    }
}
