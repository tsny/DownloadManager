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
using System.IO;


namespace DownloadManager
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        string NewDefaultPath;
        bool DefaultPathChanged;

        public Settings()
        {
            InitializeComponent();
            TB_DefPath.Text = Properties.Settings.Default.DefaultPath;
        }

        private void BTN_BrowseDefPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ValidateNames = false;
            dialog.CheckFileExists = false;
            dialog.CheckPathExists = true;
            
            dialog.FileName = "Folder Selection.";

            if(dialog.ShowDialog() == true)
            {
                NewDefaultPath = dialog.FileName;
                NewDefaultPath = System.IO.Path.GetDirectoryName(NewDefaultPath);
                TB_DefPath.Text = NewDefaultPath;
                DefaultPathChanged = true;
            }
        }

        private void BTN_Save_Click(object sender, RoutedEventArgs e)
        {
            if(DefaultPathChanged)
            {
                Properties.Settings.Default.DefaultPath = NewDefaultPath;
                Properties.Settings.Default.Save();
            }
            
        }
    }
}
