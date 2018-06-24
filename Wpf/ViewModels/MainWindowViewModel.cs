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
        private BaseCrawler crawler;

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


        public ICommand DoWorkCommand { get; private set; }
        public ICommand SetCrawlerCommand { get; private set; }

        public MainWindowViewModel()
        {
            this.progressText = new StringBuilder();
            this.crawler = new RsSandanski();
            this.DoWorkCommand = new DelegateCommand(
                async () => await Task.Run(() => this.crawler.Start(this.ReportProgress)));
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
