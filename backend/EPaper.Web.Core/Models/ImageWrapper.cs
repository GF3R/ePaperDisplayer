using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPaper.Web.Core.Models
{
    public class ImageWrapper
    {
        public string Base64 { get; set; }

        public string Bytes { get; set; }
        public int NumberOfBytes { get; set; }
        public int Depth { get; set; }
        public int FileSize { get; set; }
        public int CreatorByte { get; set; }
        public int ImageOffset { get; set; }
        public int HeaderSize { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Planes { get; set; }
        public int Format { get; set; }
    }
}
