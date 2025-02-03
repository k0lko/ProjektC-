using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using NoteApp.Models;
using NoteApp.Services;

public class NoteController : Controller
{
    private readonly INoteService _noteService;
    private readonly IWebHostEnvironment _environment;

    public NoteController(INoteService noteService, IWebHostEnvironment environment)
    {
        _noteService = noteService;
        _environment = environment;
    }

    public IActionResult Index()
    {
        var notes = _noteService.GetAllNotes();
        return View(notes);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Note note)
    {
        if (ModelState.IsValid)
        {
            _noteService.AddNote(note);
            return RedirectToAction("Index");
        }
        return View(note);
    }

    public IActionResult Edit(int id)
    {
        var note = _noteService.GetNoteById(id);
        if (note == null)
        {
            return NotFound();
        }
        return View(note);
    }

    [HttpPost]
    public IActionResult Edit(Note note)
    {
        if (ModelState.IsValid)
        {
            _noteService.UpdateNote(note);
            return RedirectToAction("Index");
        }
        return View(note);
    }

    public IActionResult Delete(int id)
    {
        var note = _noteService.GetNoteById(id);
        if (note == null)
        {
            return NotFound();
        }
        return View(note);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        _noteService.DeleteNote(id);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile image)
    {
        if (image != null && image.Length > 0)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            var ocrText = _noteService.PerformOcr(filePath);

            return Json(new { imageUrl = "~/uploads/" + uniqueFileName, ocrText = ocrText });
        }

        return BadRequest("No image uploaded.");
    }
}