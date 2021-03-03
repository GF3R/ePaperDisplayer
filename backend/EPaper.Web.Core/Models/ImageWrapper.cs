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
    }
}
