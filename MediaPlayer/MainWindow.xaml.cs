using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Media> mediaList = new List<Media>();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string img = "Images/musical-note-64x64.png";
            string img2 = "Images/film-64x64.png";

            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            mediaList.Add(new Media(currentDir + "cruel-summer1.mp3", 644000.45, img));
            mediaList.Add(new Media(currentDir + "cruel-summer2.mp3", 100000.5,img2));
            mediaList.Add(new Media(currentDir + "cruel-summer3.mp3", 6000.8, img));
            mediaList.Add(new Media(currentDir + "cruel-summer4.mp3", 279.25, img2));
            mediaList.Add(new Media(currentDir + "cruel-summer5.mp3", 103000.6, img2));
            mediaList.Add(new Media(currentDir + "cruel-summer6.mp3", 644000.45, img));
            mediaList.Add(new Media(currentDir + "cruel-summer7.mp3", 100000.5, img2));
            mediaList.Add(new Media(currentDir + "cruel-summer8.mp3", 6000.8, img));
            mediaList.Add(new Media(currentDir + "cruel-summer9.mp3", 279.25, img2));
            mediaList.Add(new Media(currentDir + "cruel-summer10.mp3", 103000.6, img2));
            mediaList.Add(new Media(currentDir + "cruel-summer11.mp3", 644000.45, img));
            mediaList.Add(new Media(currentDir + "cruel-summer12.mp3", 644000.45, img));
            plListView.ItemsSource = mediaList;
        }
    }
}
