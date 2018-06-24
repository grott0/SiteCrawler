namespace Wpf.Crawlers
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using Wpf.Models;

    public class SAC : BaseCrawler
    {
        public SAC() : base(){ }

        public override void Start(Action<string> reportProgress)
        {
            this.CrawlPages();
            reportProgress("hello world!");
        }

        public void CrawlPages()
        {
            for (int year = 2014; year <= DateTime.Now.Year; year++)
            {
                for (int month = 1; month <= 12; month++)
                {
                    for (int day = 1; day <= DateTime.DaysInMonth(year, month); day++)
                    {
                        string url = $"http://www.sac.justice.bg/court22.nsf/($All)/?SearchView&SearchWV=0&SearchFuzzy=0&Query=(FIELD%20form=Decision3%20OR%20FIELD%20form=Decision)%20AND%20(FIELD%20d_Date>={day}/{month}/{year}%20AND%20FIELD%20d_Date<{day + 1}/{month}/{year})&TIMESCOPE=1";
                        this.CrawlPage(url);
                    }
                }
            }
        }

        public void CrawlPage(string url)
        {
            using (WebClient client = new WebClient())
            {
                // Character set for the link list pages - UTF8
                client.Encoding = Encoding.UTF8;
                string pageHtml = client.DownloadString(url);
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(pageHtml);

                IEnumerable<HtmlNode> contentRows = htmlDoc
                    .DocumentNode
                    .SelectNodes("//div[@id = 'body-content']/table[1]/tr")
                    .Skip(1);

                if (contentRows.Count() > 249)
                {
                    // For debugging purposes.
                }

                foreach (HtmlNode contentRow in contentRows)
                {
                    DocumentDTO actDocument = this.GetActDocument(contentRow);

                    // Save to DB.
                }
            }
        }

        public DocumentDTO GetActDocument(HtmlNode contentRow)
        {
            using (WebClient client = new WebClient())
            {
                HtmlNode linkNode = contentRow.SelectSingleNode("./td[5]/table/tr/td[2]/font/a");
                string actUrl = "http://www.sac.justice.bg/" + linkNode.Attributes["href"].Value;

                // Character set for the act pages - KOI8-R
                client.Encoding = Encoding.GetEncoding(20866);
                string actPageHtml = client.DownloadString(actUrl);

                HtmlDocument actPageHtmlDocument = new HtmlDocument();
                actPageHtmlDocument.LoadHtml(actPageHtml);

                string actContent = actPageHtmlDocument
                    .DocumentNode
                    .SelectSingleNode("//div[@id='body-content']")
                    .OuterHtml;

                actContent = this.ConvertFromKOI8RToUTF8(actContent);

                string name = this.ExtractUniqueIdFromUrl(actUrl) + "_act.html";

                return new DocumentDTO
                {
                    Name = name,
                    Encoding = EncodingType.Utf8,
                    DataContent = Encoding.UTF8.GetBytes(actContent),
                    Format = FormatType.Html,
                    Url = actUrl
                };
            }
        }

        public string ConvertFromKOI8RToUTF8(string data)
        {
            Encoding koi8r = Encoding.GetEncoding(20866);
            byte[] koi8rBytes = koi8r.GetBytes(data);
            byte[] utf8Bytes = Encoding.Convert(koi8r, Encoding.UTF8, koi8rBytes);

            return Encoding.UTF8.GetString(utf8Bytes);
        }

        private string ExtractUniqueIdFromUrl(string url)
        {
            string id = url
                .Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Last();

            return id.Replace("?OpenDocument", "");
        }

    }
}
