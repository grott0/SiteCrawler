namespace Wpf.Crawlers
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text.RegularExpressions;
    using Wpf.Data;
    using Wpf.Models;

    /// <summary>
    /// Name: Районен съд - Сандански
    /// Url: http://www.rs-smolian.com/
    /// </summary>
    public class Sandanski : BaseCrawler
    {
        public override void Start(Action<string> reportProgress)
        {
            base.reportProgress = reportProgress;
            this.Crawl(reportProgress);
        }

        private void Crawl(Action<string> reportProgress)
        {
            base.watch.Start();
            base.reportProgress("Crawler for www.rs-sandanski.com started.");
            string baseUrl = @"http://rs-sandanski.com/";

            using (WebClient client = new WebClient())
            {
                string initialUrl = baseUrl + "verdicts.php";
                string page = client.DownloadString(initialUrl);
                string regexPattern = "<td><a href=\"([^\"]*?)\"[\\w\\W]*?</td>\\s*<td>([^<]*?)</td>\\s*<td>([^<]*?)</td>\\s*<td>([^/]*?)/([^<]*?)</td>";

                foreach (Match match in Regex.Matches(page, regexPattern, RegexOptions.IgnoreCase))
                {
                    var href = match.Groups[1].Value;
                    string url = baseUrl + href.Substring(2);

                    string fileName = Path.GetFileNameWithoutExtension(href);
                    string fileExtension = Path.GetExtension(href);
                    fileName = $"{fileName}{fileExtension}";
                    byte[] dataContent = client.DownloadData(url);
                    int? format = (int?)this.GetDataFormat(fileName);

                    Document doc = new Document
                    {
                        Name = fileName,
                        Url = url,
                        Format = format,
                        DataContent = dataContent,
                        Encoding = (int?)EncodingType.Windows1251,
                        Md5 = MD5.Create().ComputeHash(dataContent)
                    };

                    using (IRepository<Document> repository = new DocumentRepository())
                    {
                        repository.Add(doc);
                        repository.Save();
                    }

                    base.documentsDownloaded++;

                    reportProgress($"Downloaded {base.documentsDownloaded}" +
                        $" documents in {base.watch.Elapsed}");

                }

                base.watch.Stop();
                reportProgress($"Downloading finished in {base.watch.Elapsed}.");
                reportProgress($"Total documents downloaded: {base.documentsDownloaded}");
            }

        }
    }
}
