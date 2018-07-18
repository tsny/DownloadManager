using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace DownloadManager
{
    /// <summary>
    /// Interaction logic for AddURL.xaml
    /// </summary>
    public partial class AddURL : Window
    {
        string DownloadPath;
        MainWindow MainWindow;

        public AddURL(MainWindow MainWindow)
        {
            InitializeComponent();
            this.MainWindow = MainWindow;
            TB_DownloadPath.Text = Properties.Settings.Default.DefaultPath;
            DownloadPath = Properties.Settings.Default.DefaultPath;
        }

        private void BTN_Browse_Click(object sender, RoutedEventArgs e)
        {
            // Hackish way of forcing the user to select a folder (TODO: Find less hackish way)

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ValidateNames = false;
            dialog.CheckFileExists = false;
            dialog.CheckPathExists = true;

            dialog.FileName = "Folder Selection.";

            if (dialog.ShowDialog() == true)
            {
                DownloadPath = dialog.FileName;
                DownloadPath = System.IO.Path.GetDirectoryName(DownloadPath);
                TB_DownloadPath.Text = DownloadPath;
                Console.WriteLine("New path is : " + DownloadPath);
            }
        }

        private void TB_URL_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Make sure the URL is valid before enabling the OK Button

            Uri uriResult;
            bool isValid;

            isValid = Uri.TryCreate(TB_URL.Text, UriKind.Absolute, out uriResult);
            BTN_OK.IsEnabled = isValid;
        }

        private void BTN_OK_Click(object sender, RoutedEventArgs e)
        {
            Uri result;

            if (Uri.TryCreate(TB_URL.Text, UriKind.Absolute, out result))
            {
                if(Download.GetFilesize(result) <= 0)
                {
                    MessageBox.Show("No downloadable file at this URL...", "Invalid URL");
                }
                else
                {
                    MainWindow.Downloads.Add(new Download(TB_URL.Text, DownloadPath, MainWindow.Downloads));
                }
            }
        }
    }
}
