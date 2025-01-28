using System;
using System.Collections.Generic;

namespace NotatkiOCR
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public byte[] Image { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}