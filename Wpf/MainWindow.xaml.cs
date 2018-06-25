using System.Collections.Generic;
using System.Windows;
using Wpf.Crawlers;
using Wpf.ViewModels;

namespace Wpf
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            List<string> crawlers = new List<string>()
            {
                "www.rs-sandanski.com",
                "www.ac-smolian.org",
                "www.sac.justice.bg"
            };

            this.DataContext = new MainWindowViewModel(crawlers);
            InitializeComponent();
        }
    }
}
