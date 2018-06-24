namespace Wpf.Models
{
    public class DocumentDTO   
    {
        public byte[] DataContent { get; set; }
        public FormatType Format { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public byte[] Md5 { get; set; }
        public EncodingType? Encoding { get; set; } = null;
    }
}
