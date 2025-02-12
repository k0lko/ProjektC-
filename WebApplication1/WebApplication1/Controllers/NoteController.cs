using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using NoteApp.Models;
using NoteApp.Services;
using System.Collections.Generic;
using Newtonsoft.Json; // Dodajemy using dla List<string>


[Route("Note")]
    public class NoteController : Controller
    {
        private readonly INoteService _noteService;
        private readonly IWebHostEnvironment _environment;

        public NoteController(INoteService noteService, IWebHostEnvironment environment)
        {
            _noteService = noteService;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("GetNotes")]
        public IActionResult GetNotes()
        {
            var notes = _noteService.GetAllNotes();
            return Json(notes);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        public IActionResult Create(Note note, List<string> images, List<string> links)
        {
            if (ModelState.IsValid)
            {
                note.Images = images ?? new List<string>();
                note.Links = links ?? new List<string>();

                _noteService.AddNote(note);
                return RedirectToAction("Index");
            }
            return View(note);
        }

        [HttpGet("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var note = _noteService.GetNoteById(id);
            if (note == null)
            {
                return NotFound();
            }
            return View(note);
        }

        [HttpPost("Edit/{id}")]
        public IActionResult Edit(int id, Note note, List<string> images, List<string> links)
        {
            if (ModelState.IsValid)
            {
                note.Images = images ?? new List<string>();
                note.Links = links ?? new List<string>();
                _noteService.UpdateNote(note);
                return RedirectToAction("Index");
            }
            return View(note);
        }

        [HttpGet("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var note = _noteService.GetNoteById(id);
            if (note == null)
            {
                return NotFound();
            }
            return View(note);
        }

        [HttpPost("Delete/{id}"), ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _noteService.DeleteNote(id);
            return RedirectToAction("Index");
        }

        [HttpPost("UploadImage")]
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

                return Json(new { imageUrl = "/uploads/" + uniqueFileName, ocrText = ocrText });
            }

            return BadRequest("No image uploaded.");
        }

        [HttpPost("UploadAndSaveFile")]
        public async Task<IActionResult> UploadAndSaveFile(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var projectRootPath = _environment.ContentRootPath;
                var notesFolder = Path.Combine(projectRootPath, "Notes");

                if (!Directory.Exists(notesFolder))
                {
                    Directory.CreateDirectory(notesFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var filePath = Path.Combine(notesFolder, uniqueFileName);

                try
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    return Json(new
                    {
                        filePath = "/Notes/" + uniqueFileName,
                        fullPath = filePath,
                        fileName = file.FileName,
                        fileType = file.ContentType,
                        fileSize = file.Length
                    });

                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error saving file: {ex.Message}");
                }
            }

            return BadRequest("No file uploaded.");
        }
    }