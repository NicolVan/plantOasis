using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantOasis.lib.Models.Pdf
{
    public class PdfFilePrintResult
    {
        public string FileName { get; private set; }
        public byte[] FileContent { get; private set; }

        public PdfFilePrintResult(string fileName, byte[] fileContent)
        {
            this.FileName = fileName;
            this.FileContent = fileContent;
        }
    }
}

