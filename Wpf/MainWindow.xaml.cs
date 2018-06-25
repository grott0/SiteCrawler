using System;
using System.Windows;
using System.Windows.Controls;
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
            this.DataContext = new MainWindowViewModel();
            InitializeComponent();
        }

        private void UrlsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string url = this.UrlsComboBox.SelectedItem.ToString();

            switch (url)
            {
                case "www.sac.justice.bg":
                    ((MainWindowViewModel)this.DataContext).Crawler = new SAC();
                    break;
                case "www.rs-sandanski.com":
                    ((MainWindowViewModel)this.DataContext).Crawler = new Sandanski();
                    break;
                case "www.ac-smolian.org":
                    ((MainWindowViewModel)this.DataContext).Crawler = new Smolian();
                    break;
                default:
                    throw new Exception("Invalid selection.");
            }
        }
    }
}
