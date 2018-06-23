using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf.Models
{
    public class DocumentDTO   
    {
        public byte[] DataContent { get; set; }
        public FormatType Format { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Md5 { get; set; }
        public EncodingType? Encoding { get; set; } = null;
    }
}
