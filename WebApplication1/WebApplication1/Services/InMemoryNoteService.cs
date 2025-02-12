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
            EnsureDummyData();
        }

        public List<Note> GetAllNotes()
        {
            Console.WriteLine($"Zwracanie { _notes.Count } notatek.");
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
                using (var engine = new TesseractEngine("./tessdata", "pol", EngineMode.Default))
                using (var img = Pix.LoadFromFile(imagePath))
                using (var page = engine.Process(img))
                {
                    return page.GetText();
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
            Console.WriteLine($"Ładowanie notatek z folderu: {_notesFolderPath}");

            if (Directory.Exists(_notesFolderPath))
            {
                var files = Directory.GetFiles(_notesFolderPath, "*.json");
                Console.WriteLine($"Znaleziono plików: {files.Length}");

                foreach (var filePath in files)
                {
                    try
                    {
                        Console.WriteLine($"Wczytywanie pliku: {filePath}");
                        string json = File.ReadAllText(filePath);
                        var note = JsonConvert.DeserializeObject<Note>(json);

                        if (note != null)
                        {
                            _notes.Add(note);
                            Console.WriteLine($"Dodano notatkę: {note.Title}");
                        }
                        else
                        {
                            Console.WriteLine($"Błąd deserializacji: {filePath}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Błąd wczytywania pliku {filePath}: {ex.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Folder z notatkami nie istnieje.");
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
                Console.WriteLine($"Error saving note to file {filePath}: {ex.Message}");
            }
        }

        private void DeleteNoteFile(int noteId)
        {
            string filePath = Path.Combine(_notesFolderPath, $"{noteId}.json");
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting note file {filePath}: {ex.Message}");
                }
            }
        }

        private void EnsureDummyData()
        {
            if (!_notes.Any())
            {
                Console.WriteLine("Brak notatek. Tworzenie przykładowych...");

                var sampleNotes = new List<Note>
                {
                    new Note { Id = 1, Title = "Pierwsza notatka", Content = "To jest przykładowa notatka testowa.", Images = new List<string> { "/uploads/image1.jpg" }, Links = new List<string> { "https://example.com" }, OcrText = "Przetworzony tekst OCR" },
                    new Note { Id = 2, Title = "Druga notatka", Content = "Kolejna notatka do testów. Sprawdź czy się ładuje.", Images = new List<string>(), Links = new List<string> { "https://google.com" }, OcrText = "OCR nie wykrył tekstu." },
                    new Note { Id = 3, Title = "Notatka o kodowaniu", Content = "Kodowanie w .NET jest super! 🖥️", Images = new List<string> { "/uploads/code1.png", "/uploads/code2.png" }, Links = new List<string> { "https://learn.microsoft.com/en-us/dotnet/" }, OcrText = "Kodowanie jest super!" }
                };

                foreach (var note in sampleNotes)
                {
                    SaveNote(note);
                    _notes.Add(note);
                }

                Console.WriteLine("Przykładowe notatki dodane.");
            }
        }
    }
}
