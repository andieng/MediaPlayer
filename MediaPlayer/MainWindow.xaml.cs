using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using WMPLib;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Path = System.IO.Path;
using System.Net;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace MediaPlayer
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Media> mediaList = new ObservableCollection<Media>();
        string thumbnail_audio = "Images/musical-note-64x64.png";
        string thumbnail_video = "Images/film-64x64.png";
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            mediaList.Add(new Media(currentDir + "cruel-summer1.mp3", 644000.45, thumbnail_audio));
            mediaList.Add(new Media(currentDir + "cruel-summer2.mp3", 100, thumbnail_video));

            plListView.ItemsSource = mediaList;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Choose media files";
            openFileDialog.Filter = "Media files|*.mp3;*.mp4;*.wav;*.flac;*.ogg;*.avi;*.mkv|All files|*.*";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                string[] selectedFilePaths = openFileDialog.FileNames; 

                foreach (string selectedFilePath in selectedFilePaths)
                {
                    var player = new WindowsMediaPlayer();
                    var clip = player.newMedia(selectedFilePath);

                    string extension = Path.GetExtension(selectedFilePath).ToLower();

                    bool fileExists = mediaList.Any(media => media.FilePath == selectedFilePath);

                    if (!fileExists)
                    {
                        if (extension == ".mp3" || extension == ".flac" || extension == ".ogg" || extension == ".wav")
                        {
                            mediaList.Add(new Media(selectedFilePath, clip.duration, thumbnail_audio));
                        }
                        else if (extension == ".mp4" || extension == ".avi" || extension == ".mkv")
                        {
                            mediaList.Add(new Media(selectedFilePath, clip.duration, thumbnail_video));
                        }
                        else
                        {
                            // Xử lý ngoại lệ
                        }
                    }
                }
            }
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.Tag is Media media)
                {
                    mediaList.Remove(media);
                }
            }
        }

        private void OnDetailClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("detail");
        }
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select a Folder"
            };

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string selectedFolderPath = folderDialog.FileName;
                string newFolderName = "MyMediaFolder";
                string newFolderPath = Path.Combine(selectedFolderPath, newFolderName);

                if (!Directory.Exists(newFolderPath))
                {
                    Directory.CreateDirectory(newFolderPath);
                }
                foreach (var media in mediaList)
                {
                    string fileName = Path.GetFileName(media.FilePath);
                    string filePath = Path.Combine(newFolderPath, fileName);
                    WebClient webClient = new WebClient();
                    try
                    {
                        webClient.DownloadFile(media.Source.AbsoluteUri, filePath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error downloading file: {ex.Message}");
                    }
                }
            }


              
        }




        private void addListPathFile(string[] selectedFilePaths)
        {
       

                foreach (string selectedFilePath in selectedFilePaths)
                {
                    var player = new WindowsMediaPlayer();
                    var clip = player.newMedia(selectedFilePath);

                    string extension = Path.GetExtension(selectedFilePath).ToLower();

                    bool fileExists = mediaList.Any(media => media.FilePath == selectedFilePath);

                    if (!fileExists)
                    {
                        if (extension == ".mp3" || extension == ".flac" || extension == ".ogg" || extension == ".wav")
                        {
                            mediaList.Add(new Media(selectedFilePath, clip.duration, thumbnail_audio));
                        }
                        else if (extension == ".mp4" || extension == ".avi" || extension == ".mkv")
                        {
                            mediaList.Add(new Media(selectedFilePath, clip.duration, thumbnail_video));
                        }
                        else
                        {
                            // Xử lý ngoại lệ
                        }
                    }
                }
        }
        private void importButton_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select a Folder"
            };

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                mediaList.Clear();
                string selectedFolderPath = folderDialog.FileName;
                string[] files = Directory.GetFiles(selectedFolderPath, "*", SearchOption.AllDirectories);
                addListPathFile(files);
            }
        }

    }
}
