using System.Collections.Generic;
using NoteApp.Models;

namespace NoteApp.Services
{
    public interface INoteService
    {
        List<Note> GetAllNotes();
        Note GetNoteById(int id);
        void AddNote(Note note);
        void UpdateNote(Note note);
        void DeleteNote(int id);
        string PerformOcr(string imagePath);
        void LoadNotes();
    }
}