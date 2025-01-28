using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Windows.Forms;

namespace NotatkiOCR
{
    public static class PdfExporter
    {
        public static void ExportToPdf(string content)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog { Filter = "PDF Files|*.pdf" };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    Document pdfDoc = new Document();
                    PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();
                    pdfDoc.Add(new Paragraph(content));
                    pdfDoc.Close();
                }
            }
        }
    }
}