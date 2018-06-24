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

        public override void Start(Action callback)
        {
            this.Crawl();
            callback();
        }

        private void Crawl()
        {
            string baseUrl = @"http://rs-sandanski.com/";

            using (WebClient client = new WebClient())
            {
                int cnt = 0;
                string initialUrl = baseUrl + "verdicts.php";
                string page = client.DownloadString(initialUrl);
                string regexPattern = "<td><a href=\"([^\"]*?)\"[\\w\\W]*?</td>\\s*<td>([^<]*?)</td>\\s*<td>([^<]*?)</td>\\s*<td>([^/]*?)/([^<]*?)</td>";

                foreach (Match match in Regex.Matches(page, regexPattern, RegexOptions.IgnoreCase))
                {
                    var fileHref = match.Groups[1].Value;
                    string url = baseUrl + fileHref.Substring(2);

                    string fileDir = Path.GetFileNameWithoutExtension(fileHref);
                    string fileExtension = Path.GetExtension(fileHref);
                    string fileName = $"{fileDir}{fileExtension}";
                    byte[] data = client.DownloadData(url);
                    int? format = (int?)this.GetDataFormat(fileName);
                    Document doc = new Document
                    {
                        Name = fileName,
                        Url = url,
                        Format = (int?)this.GetDataFormat(fileName),
                        DataContent = data,
                        Encoding = (int?)EncodingType.Windows1251,
                        Md5 = MD5.Create().ComputeHash(data)
                    };

                    using (IRepository<Document> repository = new DocumentRepository())
                    {
                        repository.Add(doc);
                        repository.Save();
                    }
                    
                    cnt++;
                }

            }

        }
    }
}
