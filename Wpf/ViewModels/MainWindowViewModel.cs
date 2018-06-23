namespace Wpf.ViewModels
{
    using Prism.Commands;
    using Prism.Mvvm;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using Wpf.Crawlers;

    public class MainWindowViewModel : BindableBase
    {
        private BaseCrawler crawler;
        private bool completed = false;
        public bool Completed
        {
            get
            {
                return this.completed;
            }
            set
            {
                this.completed = value;
                this.RaisePropertyChanged("Completed");
            }
        }

        public ICommand DoWorkCommand { get; private set; }

        public MainWindowViewModel()
        {
            this.crawler = new RsSandanski();
            this.DoWorkCommand = new DelegateCommand(
                async () => await Task.Run(() => this.crawler.Start(this.CrawlerFinished)));
        }

        public void CrawlerFinished()
        {
            this.Completed = true;
        }


    }
}
