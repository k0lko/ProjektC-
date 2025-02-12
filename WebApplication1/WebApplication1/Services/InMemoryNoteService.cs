using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Tesseract;
using NoteApp.Models;
using Microsoft.AspNetCore.Hosting;

namespace NoteApp.Services
{
    public class InMemoryNoteService : INoteService
    {
        private List<Note> _notes = new List<Note>();
        private int _nextId = 1;
        private readonly string _notesFolderPath;
        private readonly IWebHostEnvironment _environment;

        public InMemoryNoteService(IWebHostEnvironment environment)
        {
            _environment = environment;
            _notesFolderPath = Path.Combine(_environment.ContentRootPath, "Notes");
            Directory.CreateDirectory(_notesFolderPath);
            LoadNotes();
        }

        public List<Note> GetAllNotes()
        {
            return _notes;
        }

        public Note GetNoteById(int id)
        {
            return _notes.FirstOrDefault(n => n.Id == id);
        }

        public void AddNote(Note note)
        {
            note.Id = _nextId++;
            _notes.Add(note);
            SaveNote(note);
        }

        public void UpdateNote(Note note)
        {
            var existingNote = GetNoteById(note.Id);
            if (existingNote != null)
            {
                existingNote.Title = note.Title;
                existingNote.Content = note.Content;
                existingNote.Images = note.Images;
                existingNote.Links = note.Links;
                existingNote.OcrText = note.OcrText;
                SaveNote(existingNote);
            }
        }

        public void DeleteNote(int id)
        {
            var noteToRemove = GetNoteById(id);
            if (noteToRemove != null)
            {
                _notes.Remove(noteToRemove);
                DeleteNoteFile(id);
            }
        }

        public string PerformOcr(string imagePath)
        {
            try
            {
                using (var engine = new TesseractEngine(@"./tessdata", "pol", EngineMode.Default)) // Correct way
                {
                    using (var img = Pix.LoadFromFile(imagePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            return page.GetText();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OCR Error: {ex.Message}");
                return "OCR failed.";
            }
        }


        public void LoadNotes()
        {
            _notes.Clear();
            if (Directory.Exists(_notesFolderPath)) // Check if the directory exists
            {
                foreach (var filePath in Directory.GetFiles(_notesFolderPath, "*.json"))
                {
                    try
                    {
                        string json = File.ReadAllText(filePath);
                        var note = JsonConvert.DeserializeObject<Note>(json);
                        if (note != null) // Check for null after deserialization
                        {
                            _notes.Add(note);
                        }
                        else
                        {
                           Console.WriteLine($"Error deserializing note from file: {filePath}");
                        }

                    }
                    catch (Exception ex)
                    {
                         Console.WriteLine($"Error loading note from file: {filePath}: {ex.Message}");
                    }
                }
            }
            _nextId = _notes.Any() ? _notes.Max(n => n.Id) + 1 : 1;
        }

        private void SaveNote(Note note)
        {
            string filePath = Path.Combine(_notesFolderPath, $"{note.Id}.json");
            string json = JsonConvert.SerializeObject(note, Formatting.Indented);
            try
            {
               File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving note to file: {filePath}: {ex.Message}");
            }
        }

        private void DeleteNoteFile(int noteId)
        {
            string filePath = Path.Combine(_notesFolderPath, $"{noteId}.json");
            if (File.Exists(filePath))
            {
                try{
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting note file: {filePath}: {ex.Message}");
                }
            }
        }
    }
}