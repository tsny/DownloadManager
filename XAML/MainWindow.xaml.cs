using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DownloadManager
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Download> downloads;

        public ObservableCollection<Download> Downloads
        {
            get { return downloads; }
            set
            {
                downloads = value;
            }
        }

        public MainWindow()
        {
            Downloads = new ObservableCollection<Download>();
            InitializeComponent();
            DownloadList.ItemsSource = Downloads;
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings Settings = new Settings();
            Settings.ShowDialog();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            Help help = new Help();
            help.ShowDialog();
            
        }

        private void AddURL_Click(object sender, RoutedEventArgs e)
        {
            AddURL URL_Window = new AddURL(this);
            URL_Window.ShowDialog();
        }

        private void listDownloads_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BTN_ClearDownloads_Click(object sender, RoutedEventArgs e)
        {
            foreach(Download download in downloads.ToList())
            {
                if(download.Complete)
                {
                    downloads.Remove(download);
                }
            }
        }

        private void BTN_PauseDownloads_Click(object sender, RoutedEventArgs e)
        {
            foreach (Download download in downloads.ToList())
            {
                download.PauseDownload();
            }
        }

        private void BTN_ResumeDownloads_Click(object sender, RoutedEventArgs e)
        {
            foreach (Download download in downloads.ToList())
            {
                if (download.Paused)
                {
                    download.StartDownload();
                }
            }
        }
    }
}
