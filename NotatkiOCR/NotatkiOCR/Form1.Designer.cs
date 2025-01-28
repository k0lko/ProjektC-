using System;
using System.Linq;
using System.Windows.Forms;

namespace NotatkiOCR
{
    public partial class MainForm : Form
    {
        private NotesDbContext dbContext = new NotesDbContext();
        private ListBox notesListBox = new ListBox();
        private TextBox searchBox = new TextBox();
        private Button addButton = new Button { Text = "Dodaj Notatkę" };
        private Button scanButton = new Button { Text = "Skanuj OCR" };
        private Button exportButton = new Button { Text = "Eksportuj do PDF" };
        private RichTextBox contentBox = new RichTextBox();
        
        public MainForm()
        {
            InitializeComponent();
            LoadNotes();
            exportButton.Click += ExportButton_Click;
            addButton.Click += AddButton_Click;
            scanButton.Click += ScanButton_Click;
        }

        private void LoadNotes()
        {
            // Ładuje tytuły notatek do ListBoxa z bazy danych
            notesListBox.DataSource = dbContext.Notes.Select(n => n.Title).ToList();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            var note = new Note { Title = "Nowa Notatka", Content = "Treść" };
            dbContext.Notes.Add(note);
            dbContext.SaveChanges();
            LoadNotes();
        }

        private void ScanButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string extractedText = OcrHelper.ExtractTextFromImage(openFileDialog.FileName);
                contentBox.Text = extractedText;
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            PdfExporter.ExportToPdf(contentBox.Text);
        }

        // Inicjalizacja komponentów formularza
        private void InitializeComponent()
        {
            this.notesListBox = new System.Windows.Forms.ListBox();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.addButton = new System.Windows.Forms.Button();
            this.scanButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.contentBox = new System.Windows.Forms.RichTextBox();

            // 
            // notesListBox
            // 
            this.notesListBox.FormattingEnabled = true;
            this.notesListBox.Location = new System.Drawing.Point(12, 12);
            this.notesListBox.Name = "notesListBox";
            this.notesListBox.Size = new System.Drawing.Size(250, 300);
            this.notesListBox.TabIndex = 0;

            // 
            // searchBox
            // 
            this.searchBox.Location = new System.Drawing.Point(12, 318);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(250, 22);
            this.searchBox.TabIndex = 1;

            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(280, 12);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(120, 30);
            this.addButton.TabIndex = 2;
            this.addButton.Text = "Dodaj Notatkę";
            this.addButton.UseVisualStyleBackColor = true;

            // 
            // scanButton
            // 
            this.scanButton.Location = new System.Drawing.Point(280, 48);
            this.scanButton.Name = "scanButton";
            this.scanButton.Size = new System.Drawing.Size(120, 30);
            this.scanButton.TabIndex = 3;
            this.scanButton.Text = "Skanuj OCR";
            this.scanButton.UseVisualStyleBackColor = true;

            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(280, 84);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(120, 30);
            this.exportButton.TabIndex = 4;
            this.exportButton.Text = "Eksportuj do PDF";
            this.exportButton.UseVisualStyleBackColor = true;

            // 
            // contentBox
            // 
            this.contentBox.Location = new System.Drawing.Point(12, 350);
            this.contentBox.Name = "contentBox";
            this.contentBox.Size = new System.Drawing.Size(600, 100);
            this.contentBox.TabIndex = 5;

            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.notesListBox);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.scanButton);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.contentBox);
            this.Text = "Notatki OCR";
        }
    }
}
