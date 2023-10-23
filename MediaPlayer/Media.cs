using Microsoft.Graph.Models.CallRecords;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MediaPlayer
{
    public class Media : INotifyPropertyChanged
    {
        public string? FilePath { get; set; }
        public ImageSource? PreviewImage { get; set; }
        public string? FileName 
        {
            get => Path.GetFileNameWithoutExtension(FilePath);
        }    
        public string? Name { get; set; }
        public string? Type
        {
            get
            {
                string extension = Path.GetExtension(FilePath);
                if (extension == ".mp4" || extension == ".avi" || extension == ".mkv")
                { 
                    return "video";
                } 
                else if (extension == ".mp3" || extension == ".flac" || extension == ".ogg" || extension == ".wav")
                {
                    return "music";
                }
                else
                {
                    throw new ArgumentException("Invalid file", nameof(extension));
                }
            }
        }
        public Uri Source { get; set; }
        public double Duration { get; set; }
        public string DurationString 
        { 
            get => TimeSpan.FromSeconds(Duration).ToString(@"hh\:mm\:ss");
        }

        public Media(string? filePath, double duration, string? previewImage)
        {
            this.FilePath = filePath;
            this.Name = Path.GetFileNameWithoutExtension(FilePath);
            this.Duration = duration;
            string workDir = AppDomain.CurrentDomain.BaseDirectory;
            Uri uri = new Uri($"{workDir}/{previewImage}", UriKind.Absolute);
            this.PreviewImage = new BitmapImage(uri);
            this.Source = new Uri(this.FilePath, UriKind.Absolute);
        }

        public Media(string? filePath, string Name, double duration, string? previewImage)
        {
            this.FilePath = filePath;
            this.Duration = duration;
            this.Name = Name;
            string workDir = AppDomain.CurrentDomain.BaseDirectory;
            Uri uri = new Uri($"{workDir}/{previewImage}", UriKind.Absolute);
            this.PreviewImage = new BitmapImage(uri);
            this.Source = new Uri(this.FilePath, UriKind.Absolute);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
