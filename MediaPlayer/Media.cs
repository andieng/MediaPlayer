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
        public string? Name
        {
            get => Path.GetFileNameWithoutExtension(FilePath);
        }
        public string? Type
        {
            get
            {
                if (Path.GetExtension(FilePath) == ".mp4")
                {
                    return "video";
                } 
                else
                {
                    return "music";
                }
            }
        }
        public Uri Source { get; set; }
        public double Duration { get; set; }
        public string DurationString 
        { 
            get => TimeSpan.FromSeconds(Duration).ToString(@"hh\:mm\:ss");
        }

        public Media(string? filePath, double duration, ImageSource? previewImage)
        {
            this.FilePath = filePath;
            this.Duration = duration;
            this.PreviewImage = previewImage;
        }

        public Media(string? filePath, double duration, string? previewImage)
        {
            this.FilePath = filePath;
            this.Duration = duration;
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
