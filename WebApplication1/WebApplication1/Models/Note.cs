namespace NoteApp.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public List<string> Links { get; set; } = new List<string>();
        public string OcrText { get; set; }
    }
}