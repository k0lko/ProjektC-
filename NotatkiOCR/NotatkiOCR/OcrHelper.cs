using System;
using Tesseract;

namespace NotatkiOCR
{
    public static class OcrHelper
    {
        public static string ExtractTextFromImage(string imagePath)
        {
            using (var engine = new TesseractEngine("./tessdata", "pol", EngineMode.Default))
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
    }
}