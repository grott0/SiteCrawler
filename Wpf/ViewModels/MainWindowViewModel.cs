namespace Wpf.ViewModels
{
    using Prism.Commands;
    using Prism.Mvvm;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using Wpf.Crawlers;

    public class MainWindowViewModel : BindableBase
    {
        public BaseCrawler Crawler { get; set; }
        public List<string> WebsiteUrls { get; set; }

        private bool urlsComboBoxEnabled;

        public bool UrlsComboBoxEnabled
        {
            get
            {
                return urlsComboBoxEnabled;
            }
            set
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

        public MainWindowViewModel()
        {
            this.StartButtonEnabled = true;
            this.WebsiteUrls = new List<string>()
            {
                "www.sac.justice.bg",
                "www.rs-sandanski.com",
                "www.ac-smolian.org"
            };

            this.UrlsComboBoxEnabled = true;
            this.ProgressText = new StringBuilder();
            this.StartCrawlerCommand = new DelegateCommand(
                async () => await Task.Run(() => this.StartCrawler()));
        }

        public void StartCrawler()
        {
            if (this.Crawler == null)
            {
                return;
            }

            this.StartButtonEnabled = false;
            this.UrlsComboBoxEnabled = false;
            this.Crawler.Start(this.ReportProgress);
            this.UrlsComboBoxEnabled = true;
            this.StartButtonEnabled = true;
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
