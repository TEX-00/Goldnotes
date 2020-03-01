using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Goldnote.Data;
using Goldnote.Models;
using System.IO;
namespace Goldnote.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageModelsController : ControllerBase
    {
        private readonly ImageModelDbContext _context;

        public ImageModelsController(ImageModelDbContext context)
        {
            _context = context;
        }

        // GET: api/ImageModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ImageModel>>> GetImageModels()
        {
            return await _context.ImageModels.ToListAsync();
        }

        // GET: api/ImageModels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ImageModel>> GetImageModel(string id)
        {
            var imageModel = await _context.ImageModels.FindAsync(id);

            if (imageModel == null)
            {
                return NotFound();
            }

            return new FileStreamResult(new MemoryStream(imageModel.image),"image/png");
        }

        // PUT: api/ImageModels/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutImageModel(string id, ImageModel imageModel)
        {
            if (id != imageModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(imageModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImageModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ImageModels
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<ImageModel>> PostImageModel(ImageModel imageModel)
        {
            _context.ImageModels.Add(imageModel);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ImageModelExists(imageModel.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetImageModel", new { id = imageModel.Id }, imageModel);
        }

        // DELETE: api/ImageModels/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ImageModel>> DeleteImageModel(string id)
        {
            var imageModel = await _context.ImageModels.FindAsync(id);
            if (imageModel == null)
            {
                return NotFound();
            }

            _context.ImageModels.Remove(imageModel);
            await _context.SaveChangesAsync();

            return imageModel;
        }

        private bool ImageModelExists(string id)
        {
            return _context.ImageModels.Any(e => e.Id == id);
        }
    }
}
