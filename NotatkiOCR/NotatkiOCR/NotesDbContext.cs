using Microsoft.EntityFrameworkCore;

namespace NotatkiOCR
{
    public class NotesDbContext : DbContext
    {
        public DbSet<Note> Notes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=notes.db");
        }
    }
}