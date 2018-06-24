namespace Wpf.Crawlers
{
    using System;
    using System.IO;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text.RegularExpressions;
    using Wpf.Data;
    using Wpf.Models;

    public class RsSandanski : BaseCrawler
    {
        public RsSandanski()
        {

        }

        public override void Start(Action<string> reportProgress)
        {
            this.Crawl(reportProgress);
        }

        private void Crawl(Action<string> reportProgress)
        {
            reportProgress("Crawler RsSandanski started.");
            string baseUrl = @"http://rs-sandanski.com/";

            using (WebClient client = new WebClient())
            {
                int cnt = 0;
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
                    byte[] data = client.DownloadData(url);
                    int? format = (int?)this.GetDataFormat(fileName);

                    Document doc = new Document
                    {
                        Name = fileName,
                        Url = url,
                        Format = format,
                        DataContent = data,
                        Encoding = (int?)EncodingType.Windows1251,
                        Md5 = MD5.Create().ComputeHash(data)
                    };

                    using (IRepository<Document> repository = new DocumentRepository())
                    {
                        //repository.Add(doc);
                        //repository.Save();
                    }
                    
                    cnt++;

                    if (cnt % 100 == 0)
                    {
                        reportProgress($"Downloaded documents: {cnt}");
                    }
                }

            }

        }
    }
}
