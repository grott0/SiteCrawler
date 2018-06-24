namespace Wpf.Crawlers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text.RegularExpressions;
    using Wpf.Models;

    /// <summary>
    /// Name: Административен съд - Смолян
    /// Url: http://www.ac-smolian.org/
    /// FileTree Url: http://87.126.80.220/bcap/admc/webbcap.nsf/acts?OpenView&Start=1&Count=300&Collapse=1#1
    /// Search Url: http://www.ac-smolian.org/info/reference.html
    /// </summary>
    public class SmApS : BaseCrawler
    {
        private string initialUrl = "http://87.126.80.220/bcap/admc/webbcap.nsf/acts?OpenView&Start=1&Count=300&Collapse=1#1";

        public SmApS() { }

        public override void Start(Action<string> reportProgress)
        {
            reportProgress("Hello world!");
            //this.Crawl();
        }

        /// <summary>
        /// Starts traversing the file tree and downloads the needed document groups.
        /// </summary>
        private void Crawl()
        {
            int cnt = 0;
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
                        string pageUrl = $"http://87.126.80.220/bcap/admc/webbcap.nsf/acts?OpenView&Start=1&Count=300&Expand={i}.{j}.{k}#{i}.{j}.{k}";
                        List<string> documentUrls = this.GetDocumentUrls(pageUrl);

                        if (documentUrls.Count == 0)
                        {
                            break;
                        }

                        Console.WriteLine($"Documents downloaded: {++cnt}");
                        this.DownloadDocuments(documentUrls);
                        // Save to DB.
                    }
                }
            }
        }

        private int GetRootNodesCount()
        {
            using (WebClient client = new WebClient())
            {
                string pattern = @"<a href=";
                string page = client.DownloadString(this.initialUrl);

                return Regex.Matches(page, pattern).Count;
            }

        }


        private List<string> GetDocumentUrls(string pageUrl)
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


        /// <summary>
        /// Downloads the specified document groups.
        /// </summary>
        /// <param name="documentUrls">A collection of document group urls.</param>
        private void DownloadDocuments(List<string> documentUrls)
        {
            foreach (var documentUrl in documentUrls)
            {
                using (WebClient client = new WebClient())
                {
                    string normalizedUrl = $"http://87.126.80.220{documentUrl}";
                    string[] urlArgs = documentUrl.Split('/');
                    string docName = urlArgs[5].Replace("?OpenDocument", "");
                    byte[] dataContent = client.DownloadData(normalizedUrl);
                    //string docName = new string(docUrl.Split('/').Skip(5).First().TakeWhile(x => x != '?').ToArray());

                    DocumentDTO actInfo = new DocumentDTO()
                    {
                        Name = docName + "_act.html",
                        Url = normalizedUrl,
                        Format = this.GetDataFormat(".html"),
                        Encoding = EncodingType.Utf8,
                        DataContent = dataContent
                    };
                }

            }
        }

    }
}
