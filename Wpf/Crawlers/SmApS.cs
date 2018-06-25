namespace Wpf.Crawlers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text.RegularExpressions;
    using Wpf.Data;
    using Wpf.Models;

    /// <summary>
    /// Name: Административен съд - Смолян
    /// Url: http://www.ac-smolian.org/
    /// FileTree Url: http://87.126.80.220/bcap/admc/webbcap.nsf/acts?OpenView&Start=1&Count=300&Collapse=1#1
    /// Search Url: http://www.ac-smolian.org/info/reference.html
    /// </summary>
    public class SmApS : BaseCrawler
    {

        public override void Start(Action<string> reportProgress)
        {
            base.reportProgress = reportProgress;
            this.Crawl();
        }

        /// <summary>
        /// Starts traversing the file tree and downloads the needed document groups.
        /// </summary>
        private void Crawl()
        {
            base.watch.Start();
            base.reportProgress("Crawler for www.ac-smolian.com started.");
            int rootNodesCount = this.GetRootNodesCount();

            // First level iterator.
            for (int i = 1; i <= rootNodesCount; i++)
            {
                // Second level iterator.
                for (int j = 1; j <= 12; j++)
                {
                    // Third level iterator.
                    for (int k = 1; k <= int.MaxValue; k++)
                    {
                        string pageUrl = $"http://213.91.128.55/bcap/admc/webbcap.nsf/acts?OpenView&Start=1&Count=300&Expand={i}.{j}.{k}#{i}.{j}.{k}";
                        List<string> documentUrls = this.GetDocumentsUrls(pageUrl);

                        if (documentUrls.Count == 0)
                        {
                            break;
                        }
                        
                        this.DownloadDocuments(documentUrls);
                    }
                }
            }

            base.watch.Stop();
            base.reportProgress($"Downloading finished in {watch.Elapsed}.");
            base.reportProgress($"Total documents downloaded: {documentsDownloaded}");
        }

        private int GetRootNodesCount()
        {
            using (WebClient client = new WebClient())
            {
                string pattern = @"<a href=";
                string initialUrl = "http://213.91.128.55/bcap/admc/webbcap.nsf/acts?OpenView&Start=1&Count=300&Collapse=1#1";
                string page = client.DownloadString(initialUrl);
                int rootNodesCount = Regex.Matches(page, pattern).Count;

                return rootNodesCount;
            }

        }


        private List<string> GetDocumentsUrls(string pageUrl)
        {
            using (WebClient client = new WebClient())
            {
                string pattern = "<a href=\"(.+?OpenDocument)\">";
                string page = client.DownloadString(pageUrl);

                List<string> docUrls = new List<string>();

                foreach (Match match in Regex.Matches(page, pattern))
                {
                    docUrls.Add(match.Groups[1].Value);
                }

                return docUrls;
            }
        }

        private void DownloadDocuments(List<string> documentUrls)
        {
            foreach (var documentUrl in documentUrls)
            {
                using (WebClient client = new WebClient())
                {
                    string normalizedUrl = $"http://213.91.128.55{documentUrl}";
                    string[] urlArgs = documentUrl.Split('/');
                    string docName = urlArgs[5].Replace("?OpenDocument", "");
                    byte[] dataContent = client.DownloadData(normalizedUrl);
                    //string docName = new string(docUrl.Split('/').Skip(5).First().TakeWhile(x => x != '?').ToArray());

                    Document actDocument = new Document()
                    {
                        Name = docName + "_act.html",
                        Url = normalizedUrl,
                        Format = (int?)this.GetDataFormat(".html"),
                        Encoding = (int?)EncodingType.Utf8,
                        DataContent = dataContent,
                        Md5 = MD5.Create().ComputeHash(dataContent)
                    };

                    using (IRepository<Document> repository = new DocumentRepository())
                    {
                        repository.Add(actDocument);
                        repository.Save();
                    }


                    base.documentsDownloaded++;

                    if (base.documentsDownloaded % 100 == 0)
                    {
                        base.reportProgress($"Downloaded {base.documentsDownloaded}" +
                            $" documents in {base.watch.Elapsed}");
                    }
                }

            }
        }

    }
}
