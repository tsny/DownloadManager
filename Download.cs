using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DownloadManager
{
    public class Download : INotifyPropertyChanged
    {
        private ICommand pauseCommand;
        private ICommand startCommand;
        private ICommand deleteCommand;

        public ICommand PauseCommand
        {
            get
            {
                if (pauseCommand == null)
                {
                    pauseCommand = new RelayCommand(
                        param => PauseDownload()

                    );
                }
                return pauseCommand;
            }
        }
        public ICommand StartCommand
        {
            get
            {
                if (startCommand == null)
                {
                    startCommand = new RelayCommand(
                        param => StartDownload()
                    );
                }
                return startCommand;
            }
        }
        public ICommand DeleteCommand
        {
            get
            {
                if (deleteCommand == null)
                {
                    deleteCommand = new RelayCommand(
                        param => DeleteDownload()
                    );
                }
                return deleteCommand;
            }
        }

        private void DeleteDownload()
        {
            if (!DownloadStatus.Equals("Complete"))
            {
                string sMessageBoxText = "Attempting to delete incomplete download. Continue?";
                string sCaption = "Incomplete Download!";

                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

                switch (rsltMessageBox)
                {
                    case MessageBoxResult.Yes:
                        Downloads.Remove(this);
                        break;

                    case MessageBoxResult.No:
                        /* ... */
                        break;

                    case MessageBoxResult.Cancel:
                        /* ... */
                        break;
                }
            }
            else
            {
                Downloads.Remove(this);
            }
                
        }

        public void StartDownload()
        {
            if (!DownloadStatus.Equals("Completed"))
            {
                CanRun = true;
                DownloadStatus = "Running";
                OnNotifyPropertyChanged();
                DownloadAsync();
            }
        }


        public void PauseDownload()
        {
            if(CanRun)
            {
                CanRun = false;
                DownloadStatus = "Paused";
            }
        }

        private string filename;
        private string downloadPath;
        private double filesize;
        private int bytesWritten;
        private float downloadPercentage;
        private bool paused;
        private bool canRun;
        private bool complete;
        private Uri url;
        private DateTime startTime;
        private DateTime finishTime;
        private string filesizeString;
        private string downloadStatus;
        private ObservableCollection<Download> downloads;

        public string Filename { get => filename; set => filename = value; }
        public string DownloadPath { get => downloadPath; set => downloadPath = value; }
        public double Filesize { get => filesize; set => filesize = value; }
        public int BytesWritten { get => bytesWritten; set => bytesWritten = value; }
        public float DownloadPercentage { get => downloadPercentage; set { downloadPercentage = value; OnNotifyPropertyChanged("DownloadPercentage");} }
        public bool Paused { get => paused; set => paused = value; }
        public bool CanRun { get => canRun; set => canRun = value; }
        public bool Complete { get => complete; set => complete = value; }
        public Uri URL { get => url; set => url = value; }
        public DateTime StartTime { get => startTime; set => startTime = value; }
        public DateTime FinishTime { get => finishTime; set => finishTime = value; }
        public string FilesizeString { get => filesizeString; set => filesizeString = value; }
        public string DownloadStatus { get => downloadStatus; set { downloadStatus = value; OnNotifyPropertyChanged("DownloadStatus"); } }

        public ObservableCollection<Download> Downloads { get => downloads; set => downloads = value; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnNotifyPropertyChanged([CallerMemberName] string memberName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }

        public Download(string URL, String DownloadPath, ObservableCollection<Download> Downloads)
        {
            this.Downloads = Downloads;
            CanRun = true;
            this.DownloadPath = DownloadPath;
            StartTime = DateTime.Now;
            Uri outURL;

            if (!Uri.TryCreate(URL, UriKind.Absolute, out outURL))
            {
                MessageBox.Show("URL is invalid.");
            }
            else
            {
                this.URL = outURL;
                Filesize = GetFilesize(this.URL);
                Filename = System.IO.Path.GetFileName(URL);
                FilesizeString = Math.Round((Filesize * .000001), 2) + "MB";
                DownloadStatus = "Paused";
                Paused = true;
            }
        }

        public static double GetFilesize(Uri URL)
        {
            // Get file's size in bytes

            WebRequest request;

            try
            {
                request = (HttpWebRequest)WebRequest.Create(URL);
            }
            catch(NotSupportedException)
            {
                MessageBox.Show("Filetype is not supported.", "Unsupported filetype");
                return 0;
            }
                
            request.Method = "HEAD";

            try
            {
                using (var response = request.GetResponse())
                    return response.ContentLength;
            }
            catch (WebException)
            {
                MessageBox.Show("URL was invalid.", "Invalid URL");
                return 0;
            }
        }

        private async Task DownloadAsync(int range)
        {
            if (!CanRun)
            {
                throw new InvalidOperationException();
            }

            var request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "GET";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
            request.AddRange(range);

            using (var response = await request.GetResponseAsync())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    using (var fs = new FileStream(DownloadPath + "\\" + Filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    {
                        while (CanRun)
                        {
                            DownloadStatus = "Running";
                            var buffer = new byte[(int)Filesize];
                            var bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length);
                            
                            if (bytesRead == 0) break;
                            await fs.WriteAsync(buffer, 0, bytesRead);
                            BytesWritten += bytesRead;
                            DownloadPercentage = (float)((BytesWritten / Filesize) * 100);
                            
                        }

                        await fs.FlushAsync();

                        if(BytesWritten == (int)Filesize)
                        {
                            DownloadStatus = "Complete";
                            FinishTime = DateTime.Now;
                            Complete = true;
                            Console.WriteLine(DownloadStatus);
                            OnNotifyPropertyChanged();
                            CanRun = false;
                        }
                        
                    }
                }
            }
        }

        public Task DownloadAsync()
        {
            return DownloadAsync(BytesWritten);
        }
    }
}
