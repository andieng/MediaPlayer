using Microsoft.Win32;
using MediaPlayer.Keys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WMPLib;
using System.Collections.ObjectModel;
using Path = System.IO.Path;
using System.Net;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Media;

namespace MediaPlayer
{
    public partial class MainWindow : Window
    {
        private Media? currentMedia;
        private bool isPlayingMedia = false;
        private bool isShuffling = false;
        private DispatcherTimer timerVideoTime;
        private TimeSpan totalTime;
        private ObservableCollection<Media> mediaList = new ObservableCollection<Media>();

        string thumbnail_audio = "Images/musical-note-64x64.png";
        string thumbnail_video = "Images/film-64x64.png";

        bool isDelete = false;

        public MainWindow()
        {
            this.DataContext = this;
            InitializeComponent();

            HotkeysManager.SetupSystemHook();

            // Save hotkey
            HotkeysManager.AddHotkey(new GlobalHotkey(ModifierKeys.Control, Key.S, savePlaylist));

            // Play & Pause hotkey
            HotkeysManager.AddHotkey(new GlobalHotkey(ModifierKeys.Control, Key.P, playOrPauseMedia));

            // Skip to next media hotkey
            HotkeysManager.AddHotkey(new GlobalHotkey(ModifierKeys.Control, Key.F, playNextMedia));

            // Skip to previous media hotkey
            HotkeysManager.AddHotkey(new GlobalHotkey(ModifierKeys.Control, Key.B, playPreviousMedia));

            // Shuffle on/off hotkey
            HotkeysManager.AddHotkey(new GlobalHotkey(ModifierKeys.Control, Key.H, toggleShuffle));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            mediaList.Add(new Media(currentDir + "cruel-summer1.mp3", 644000.45, thumbnail_audio));
            mediaList.Add(new Media(currentDir + "cruel-summer2.mp3", 100, thumbnail_video));
            if (mediaList.Count == 0)
            {
                saveButton.IsEnabled = false;
            }
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

                    if (fileExists)
                    {
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(selectedFilePath);
                        string newFileName = fileNameWithoutExtension;
                        int index = 1;
                        while (mediaList.Any(media => media.Name == newFileName))
                        {
                            newFileName = $"{fileNameWithoutExtension} ({index})";
                            index++;
                        }

                        if (extension == ".mp3" || extension == ".flac" || extension == ".ogg" || extension == ".wav")
                        {
                            mediaList.Add(new Media(selectedFilePath, newFileName, clip.duration, thumbnail_audio));
                        }
                        else if (extension == ".mp4" || extension == ".avi" || extension == ".mkv")
                        {
                            mediaList.Add(new Media(selectedFilePath, newFileName, clip.duration, thumbnail_video));
                        }
                        else
                        {
                            throw new ArgumentException("Invalid file", nameof(extension));
                        }
                    } else
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
                            throw new ArgumentException("Invalid file", nameof(extension));
                        }
                    }
                }
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            savePlaylist();
        }

        private void savePlaylist()
        {
            var folderDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select a Folder"
            };

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string selectedFolderPath = folderDialog.FileName;
                string newFolderName = "";
                string newFolderPath = "";
                int i = 0;
                do
                {
                    if (i == 0)
                    {
                        newFolderName = "MyMediaFolder";
                    }
                    else
                    {
                        newFolderName = $"MyMediaFolder ({i})";
                    }
                    newFolderPath = Path.Combine(selectedFolderPath, newFolderName);
                    i++;
                } while (Directory.Exists(newFolderPath));
                Directory.CreateDirectory(newFolderPath);

                foreach (var media in mediaList)
                {
                    string extension = Path.GetExtension(media?.FilePath);
                    string fileNameWithoutExtension = media?.FileName;
                    if (media?.FileName != media?.Name)
                    {
                        fileNameWithoutExtension = media?.Name;
                    }
                    string fileName = fileNameWithoutExtension + extension;
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
                            throw new ArgumentException("Invalid file", nameof(extension));
                        }
                    }
                }
            if (mediaList.Count > 0)
            {
                saveButton.IsEnabled = true;
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

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            HotkeysManager.ShutdownSystemHook();
        }

        private void AddPresetButton_Click(object sender, RoutedEventArgs e)
        {
            var addButton = sender as FrameworkElement;
            if (addButton != null)
            {
                addButton.ContextMenu.IsOpen = true;
            }
        }

        private void plListView_SelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            if (plListView.SelectedIndex < 0)
            {
                return;
            }
            
            if (currentMedia != null)
            {
                pauseMedia();
                currentMediaElement.Close();
            } else
            {
                hideWelcomeBackground();
            }

            currentMedia = (Media)plListView.SelectedItem;
            showMediaControl();
            if (currentMedia?.Type == "music")
            {
                hideVideoBackground();
                showMusicBackground();
            }
            else
            {
                hideMusicBackground();
                showVideoBackground();
            }

            if (currentMedia?.FilePath != null)
            {
                currentMediaElement.Source = new Uri(currentMedia?.FilePath, UriKind.Relative);
                if (currentMediaElement.Source != null)
                {
                    handleMedia();
                }
            }
        }

        private void hideWelcomeBackground()
        {
            waitingBackground.Visibility = Visibility.Hidden;
            welcomeTextBlock.Visibility = Visibility.Hidden;
            guideTextBlock.Visibility = Visibility.Hidden;
        }

        private void showMusicBackground()
        {
            musicMediaBackground.Visibility = Visibility.Visible;
            musicNote.Visibility = Visibility.Visible;
        }

        private void hideMusicBackground()
        {
            musicMediaBackground.Visibility = Visibility.Hidden;
            musicNote.Visibility = Visibility.Hidden;
        }

        private void showVideoBackground()
        {
            videoMediaBackground.Visibility = Visibility.Visible;
        }

        private void hideVideoBackground()
        {
            videoMediaBackground.Visibility = Visibility.Hidden;
        }

        private void showMediaControl()
        {
            currentMediaElement.Visibility = Visibility.Visible;
            mediaControl.Visibility = Visibility.Visible;
            mediaNameTextBlock.Text = currentMedia?.Name; 
        }

        private void currentMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            timerVideoTime.Stop();
            if (!isShuffling)
            {
                if (plListView.SelectedIndex == plListView.Items.Count - 1)
                {
                    plListView.SelectedIndex = 0;
                } else
                {
                    plListView.SelectedIndex += 1;
                }
            } else
            {
                Random rand = new Random();
                int idx = rand.Next(0, plListView.Items.Count);
                plListView.SelectedIndex = idx;
            }

        }

        private void currentMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            updateTimeSlider();
        }

        private void updateTimeSlider()
        {
            totalTime = currentMediaElement.NaturalDuration.TimeSpan;

            // Create a timer that will update the counters and the time slider
            timerVideoTime = new DispatcherTimer();
            timerVideoTime.Interval = TimeSpan.FromSeconds(0.05);
            timerVideoTime.Tick += new EventHandler(timerTick);
            timerVideoTime.Start();
        }

        private void timerTick(object sender, EventArgs e)
        {
            if (totalTime.TotalSeconds > 0)
            {
                // Updating time slider
                timelineSlider.Value = currentMediaElement.Position.TotalSeconds /
                                   totalTime.TotalSeconds;
            }
        }

        private void handleMedia()
        {
            currentMediaElement?.Play();
            playMedia();
        }

        private void playMediaButton_Click(object sender, RoutedEventArgs e)
        {
            playOrPauseMedia();
        }

        private void backMediaButton_Click(object sender, RoutedEventArgs e)
        {
            playPreviousMedia();
        }

        private void playPreviousMedia()
        {
            int i = plListView.SelectedIndex;
            if (i == 0)
            {
                plListView.SelectedIndex = plListView.Items.Count - 1;
            }
            else
            {
                plListView.SelectedIndex = i - 1;
            }
        }

        private void nextMediaButton_Click(object sender, RoutedEventArgs e)
        {
            playNextMedia();
        }

        private void playNextMedia()
        {
            int i = plListView.SelectedIndex;
            if (i == plListView.Items.Count - 1)
            {
                plListView.SelectedIndex = 0;
            }
            else
            {
                plListView.SelectedIndex = i + 1;
            }
        }

        private void playOrPauseMedia()
        {
            if (isPlayingMedia)
            {
                pauseMedia();
            }
            else
            {
                playMedia();
            }
        }

        private void playMedia()
        {
            string workDir = AppDomain.CurrentDomain.BaseDirectory;
            currentMediaElement?.Play();
            isPlayingMedia = true;
            Uri uri = new Uri($"{workDir}/Images/pause.png", UriKind.Absolute);
            playMediaButtonImageSource.Source = new BitmapImage(uri);
            playMediaButtonImageSource.Margin = new Thickness(0, 0, 0, 0);
        }
        private void pauseMedia()
        {
            string workDir = AppDomain.CurrentDomain.BaseDirectory;
            currentMediaElement?.Pause();
            isPlayingMedia = false;
            Uri uri = new Uri($"{workDir}/Images/play.png", UriKind.Absolute);
            playMediaButtonImageSource.Source = new BitmapImage(uri);
            playMediaButtonImageSource.Margin = new Thickness(2, 0, 0, 0);
        }

        private void SeekToMediaPosition(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            int SliderValue = (int)timelineSlider.Value;

            // Overloaded constructor takes the arguments days, hours, minutes, seconds, milliseconds.
            // Create a TimeSpan with miliseconds equal to the slider value.
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
            currentMediaElement.Position = ts;
        }

        private void shuffleButton_Click(object sender, RoutedEventArgs e)
        {
            toggleShuffle();
        }

        private void toggleShuffle()
        {
            if (isShuffling)
            {
                isShuffling = false;
                string workDir = AppDomain.CurrentDomain.BaseDirectory;
                Uri uri = new Uri($"{workDir}/Images/shuffle.png", UriKind.Absolute);
                shuffleButtonImageSource.Source = new BitmapImage(uri);
            }
            else
            {
                isShuffling = true;
                string workDir = AppDomain.CurrentDomain.BaseDirectory;
                Uri uri = new Uri($"{workDir}/Images/shuffle-off.png", UriKind.Absolute);
                shuffleButtonImageSource.Source = new BitmapImage(uri);
            }
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.Tag is Media media)
                {
                    mediaList.Remove(media);
                }
            }
        }
    }
}
