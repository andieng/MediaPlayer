using Microsoft.Graph.Models.CallRecords;
using System;
using System.ComponentModel;
using System.IO;
//using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MediaPlayer
{
    class Media : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string? FilePath { get; set; }
        public ImageSource? PreviewImage { get; set; }
        public string? Name
        {
            get => Path.GetFileNameWithoutExtension(FilePath);
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
            string workDir = Environment.CurrentDirectory;
            string projectDir = Directory.GetParent(workDir).Parent.Parent.FullName;
            Uri uri = new Uri($"{projectDir}/{previewImage}", UriKind.Absolute);
            this.PreviewImage = new BitmapImage(uri);
            this.Source = new Uri(this.FilePath, UriKind.Absolute);
        }
    }
}
