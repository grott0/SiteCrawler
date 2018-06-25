namespace Wpf.ViewModels
{
    using Prism.Commands;
    using Prism.Mvvm;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Wpf.Crawlers;

    public class MainWindowViewModel : BindableBase
    {
        private BaseCrawler crawler;

        private string crawlerName;
        public string CrawlerName
        {
            get
            {
                return this.crawlerName;
            }
            set
            {
                this.crawlerName = value;
                this.RaisePropertyChanged("CrawlerName");
                this.SetCrawler();
            }
        }

        private List<string> crawlerNames;
        public List<string> CrawlerNames
        {
            get
            {
                return crawlerNames;
            }
            private set
            {
                crawlerNames = value;
                this.RaisePropertyChanged("CrawlerNames");
            }
        }

        private bool urlsComboBoxEnabled;

        public bool UrlsComboBoxEnabled
        {
            get
            {
                return urlsComboBoxEnabled;
            }
            private set
            {
                urlsComboBoxEnabled = value;
                this.RaisePropertyChanged("UrlsComboBoxEnabled");
            }
        }

        private bool startButtonEnabled;
        public bool StartButtonEnabled
        {
            get
            {
                return startButtonEnabled;
            }
            private set
            {
                startButtonEnabled = value;
                this.RaisePropertyChanged("StartButtonEnabled");
            }
        }

        private StringBuilder progressText;
        public StringBuilder ProgressText
        {
            get
            {
                return this.progressText;
            }
            set
            {
                this.progressText = value;
                this.RaisePropertyChanged("ProgressText");
            }
        }


        public ICommand StartCrawlerCommand { get; private set; }

        public MainWindowViewModel(List<string> crawlers)
        {
            this.crawlerNames = crawlers;
            this.StartButtonEnabled = true;
            this.UrlsComboBoxEnabled = true;
            this.ProgressText = new StringBuilder();
            this.StartCrawlerCommand = new DelegateCommand(
                async () => await Task.Run(() => this.StartCrawler()));
        }

        public void StartCrawler()
        {
            if (this.crawler == null)
            {
                return;
            }

            this.StartButtonEnabled = false;
            this.UrlsComboBoxEnabled = false;
            this.crawler.Start(this.ReportProgress);
            this.UrlsComboBoxEnabled = true;
            this.StartButtonEnabled = true;
        }


        private void SetCrawler()
        {
            switch (this.crawlerName)
            {
                case "www.rs-sandanski.com":
                    this.crawler = new Sandanski();
                    break;
                case "www.sac.justice.bg":
                    this.crawler = new SAC();
                    break;
                case "www.ac-smolian.org":
                    this.crawler = new Smolian();
                    break;
                default: throw new Exception("Invalid crawler specified.");
            }
        }


        public void ReportProgress(string line)
        {
            if (this.ProgressText.Length > 2000)
            {
                this.ProgressText.Clear();
            }

            this.ProgressText.AppendLine(line);
            this.RaisePropertyChanged("ProgressText");
        }

    }
}
