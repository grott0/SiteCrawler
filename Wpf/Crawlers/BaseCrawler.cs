namespace Wpf.Crawlers
{
    using System;
    using System.IO;
    using System.Globalization;
    using Wpf.Models;
    using System.Diagnostics;

    public abstract class BaseCrawler
    {
        protected CultureInfo cultureInfo = CultureInfo.InvariantCulture;
        protected int documentsDownloaded = 0;
        protected Stopwatch watch = new Stopwatch();
        protected Action<string> reportProgress;

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
    }
}
