namespace Wpf.Crawlers
{
    using System;
    using System.IO;
    using System.Globalization;
    using Wpf.Models;

    public abstract class BaseCrawler
    {
        protected CultureInfo cultureInfo = CultureInfo.InvariantCulture;

        public BaseCrawler()
        {

        }

        public abstract void Start(Action<string> reportProgress);

        /// <summary>
        /// Gets the data format of a file from a uri.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public FormatType GetDataFormat(string uri)
        {
            string dataFormat = Path.GetExtension(uri);

            switch (dataFormat)
            {
                case ".zip": return FormatType.Zip;
                case ".pdf": return FormatType.Pdf;
                case ".html": return FormatType.Html;
                case ".htm": return FormatType.Htm;
                case ".json": return FormatType.Json;
                case ".xml": return FormatType.Xml;
                case ".doc": return FormatType.Doc;
                case ".docx": return FormatType.Docx;
                default: return FormatType.Error;
            }
        }

        public virtual string SanitizeFilename(string filename)
        {
            filename = filename
                .Replace("/", string.Empty)
                .Replace("\\", string.Empty)
                .Replace("*", string.Empty)
                .Replace("<", string.Empty)
                .Replace(">", string.Empty)
                .Replace(":", string.Empty)
                .Replace("\"", string.Empty)
                .Replace("?", string.Empty)
                .Replace("|", string.Empty)
                .Replace("=", string.Empty)
                .Replace("amp;", string.Empty)
                .Replace("#", string.Empty)
                .Replace("[", string.Empty)
                .Replace("]", string.Empty)
                .Replace("@", string.Empty);

            filename = filename.Trim().ToLower();

            return filename;
        }

        protected string BuildHtmlDocument(string content, EncodingType encodingType)
        {
            var charset = "UTF-8";
            switch (encodingType)
            {
                case EncodingType.Utf8:
                    charset = "UTF-8";
                    break;
                case EncodingType.Windows1251:
                    charset = "windows-1251";
                    break;
                case EncodingType.Windows1252:
                    charset = "windows-1251";
                    break;
                default:
                    charset = "UTF-8";
                    break;
            }

            var res = $"<html><head><meta charset=\"{charset}\"></head><body>{content}</body></html>";
            return res;
        }
    }
}
