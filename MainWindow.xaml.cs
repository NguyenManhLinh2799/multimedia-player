﻿using Gma.System.MouseKeyHook;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace multimedia_player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        MediaPlayer Player = new MediaPlayer();
        DispatcherTimer timer;
        int currentIndex = -1;
        TimeSpan duration;
        Data dataFromFile = new Data();
        //BindingList<PlaylistObject> listOfPlaylists = new BindingList<PlaylistObject>();
        string playListFile = System.AppDomain.CurrentDomain.BaseDirectory + "playList.json";

        private IKeyboardMouseEvents _hook;

        public MainWindow()
        {
            InitializeComponent();
            Player.MediaEnded += Player_MediaEnded;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;

            //đăng kí sự kiện hook
            _hook = Hook.GlobalEvents();
            _hook.KeyUp += KeyUp_hook;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (Player.Source != null && Player.NaturalDuration.HasTimeSpan)
            {

                //var filename = FullPaths[currentIndex].Name;
                //var converter = new NameConverter();
                //var shortname = converter.Convert(filename, null, null, null);
                var converter = new NameConverter();
                var currentPos = Player.Position.ToString(@"mm\:ss");

                var duration = Player.NaturalDuration.TimeSpan.ToString(@"mm\:ss");

                currenttime.Content = $"{currentPos}/{duration}";

                int timeOfPlayer = Player.NaturalDuration.TimeSpan.Minutes * 60 + Player.NaturalDuration.TimeSpan.Seconds;
                Slider.Maximum = timeOfPlayer;
                Slider.Value += 1;
                
            }
            //else
            //    Title = "No file selected...";
        }

        int currentRandomPlay = 0;

        int countSong = 0;
        void LoopOne()
        {
            PlaySelectedIndex(currentIndex);
            Slider.Value = 0;
        }

        void Loop()
        {
            if (currentIndex < FullPaths.Count - 1)
            {
                currentIndex++;
            }
            else
            {
                currentIndex = 0;
            }

            if (Randomly)
            {
                currentRandomPlay++;
                if (currentRandomPlay <= ListRandomPlay.Length - 1)
                {
                    currentIndex = ListRandomPlay[currentRandomPlay];
                }
                else
                {
                    currentRandomPlay = 0;
                    RanDomListSong();
                }
            }

            PlaySelectedIndex(currentIndex);
            Slider.Value = 0;
        }

        void NotLoop()
        {
            countSong++;
            if (countSong >= FullPaths.Count)
            {
                Player.Stop();
                timer.Stop();
                isPausing = false;
                isPlaying = false;
                Slider.Value = 0;
                Stop();
                playPauseCheckbox.IsChecked = false;
                countSong = 0;
            }
            else
            {
                if (currentIndex < FullPaths.Count - 1)
                {
                    currentIndex++;
                }
                else
                {
                    currentIndex = 0;
                }

                if (Randomly)
                {

                    if (currentRandomPlay < ListRandomPlay.Length - 1)
                    {
                        currentRandomPlay++;
                        currentIndex = ListRandomPlay[currentRandomPlay];
                    }

                }

                PlaySelectedIndex(currentIndex);
                Slider.Value = 0;
            }
        }
        private void Player_MediaEnded(object sender, EventArgs e)
        {
            Storyboard spin = (Storyboard)FindResource("startSpin");

            spin.Stop(disk);

            //khi có lặp lại 1 bài mãi mãi
            if (isRepeatOne)
            {
                LoopOne();
            }

            //khi có lặp lại playlist mãi mãi
            if (isLoopRepeat)
            {

                Loop();
            }
            if (!isLoopRepeat && !isRepeatOne)
            {

                NotLoop();
            }
        }

        BindingList<FileInfo> FullPaths = new BindingList<FileInfo>();

        //int countCheckBox=0;
        //BindingList<int> tagListBox=new BindingList<int>();
        private void addFile_Click(object sender, RoutedEventArgs e)
        {
            var screen = new OpenFileDialog();
            screen.Multiselect = true;
            
            if (screen.ShowDialog() == true)
            {
                foreach (var file in screen.FileNames)
                {
                    var info = new FileInfo(file);
                    FullPaths.Add(info);
                }
                //CheckBox ch = new CheckBox();
                //countCheckBox++;
                //tagListBox.Add(countCheckBox);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataFromFile.listOfPlayLists = new BindingList<PlaylistObject>();
         
            string line;

            if(File.Exists(playListFile))
            {
                using (StreamReader sr = new StreamReader(playListFile))
                {
                    line = sr.ReadToEnd();
                }
                if (line != "")
                {
                    dataFromFile = JsonConvert.DeserializeObject<Data>(line);
                }

            }

            if (dataFromFile.lastTimePlaying != null)
            {
                FullPaths = dataFromFile.lastTimePlaying;
            }

            if (dataFromFile.index != -1)
            {
                ListBoxFiles.SelectedIndex = dataFromFile.index;
            }
            ListBoxPlaylist.ItemsSource = dataFromFile.listOfPlayLists;
            ListBoxFiles.ItemsSource = FullPaths;
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            Play();
        }

        void Play()
        {
            Storyboard spin = (Storyboard)FindResource("startSpin");

            if (isPausing == true)
            {
                timer.Start();
                Player.Play();
                isPlaying = true;
                spin.Resume(disk);
            }
            else
            {
                if (Randomly)
                {
                    currentIndex = ListRandomPlay[currentRandomPlay];
                    PlaySelectedIndex(currentIndex);
                }
                else
                {
                    if (ListBoxFiles.SelectedIndex >= 0)
                    {
                        currentIndex = ListBoxFiles.SelectedIndex;
                        PlaySelectedIndex(currentIndex);

                    }
                    else
                    {
                        MessageBox.Show("No file selected!");
                        playPauseCheckbox.IsChecked = false;
                    }
                }
            }
        }

        bool isPlaying = false;
        bool isPausing = false;

        private void PlaySelectedIndex(int i)
        {
            if(FullPaths.Count>0)
            {
                Storyboard spin = (Storyboard)FindResource("startSpin");
                spin.Begin(disk, true);
                string filename = FullPaths[i].FullName;
                Player.Open(new Uri(filename, UriKind.Absolute));
                Player.Play();
                isPlaying = true;
                timer.Start();
                //countSong++;
                ListBoxFiles.SelectedIndex = i;
                var lbname = FullPaths[i].Name;
                //var converter = new NameConverter();               
                var converter = new NameConverter();
                var shortname = converter.Convert(lbname, null, null, null) ;
                lbPlayer.Text = shortname.ToString();
            }
            else
            {
                Stop();
            }
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            Pause();
        }

        void Pause()
        {
            Storyboard spin = (Storyboard)FindResource("startSpin");

            if (isPlaying)
            {
                Player.Pause();
                spin.Pause(disk);
                isPausing = true;
                timer.Stop();
                isPlaying = !isPlaying;
            }
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            Previous();
        }

        void Previous()
        {
            if (Randomly)
            {
                if (currentRandomPlay > 0)
                {
                    Player.Stop();
                    Slider.Value = 0;
                    currentRandomPlay -= 1;
                    currentIndex = ListRandomPlay[currentRandomPlay];
                    PlaySelectedIndex(currentIndex);
                    //ListBoxFiles.Items.CurrentItem(currentIndex)
                }
            }
            else
            {
                if (currentIndex > 0)
                {
                    Player.Stop();
                    Slider.Value = 0;
                    currentIndex -= 1;
                    PlaySelectedIndex(currentIndex);
                    //ListBoxFiles.Items.CurrentItem(currentIndex)
                }
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            Next();
        }

        void Next()
        {
            //khi có lặp lại 1 bài mãi mãi
            if (isRepeatOne)
            {
                LoopOne();
            }

            //khi có lặp lại playlist mãi mãi
            if (isLoopRepeat)
            {
                Loop();
            }
            if (!isLoopRepeat && !isRepeatOne)
            {

                NotLoop();
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        void Stop()
        {
            Storyboard spin = (Storyboard)FindResource("startSpin");
            spin.Stop(disk);
            //spin.Pause();
            Player.Stop();
            timer.Stop();
            isPausing = false;
            isPlaying = false;
            Slider.Value = 0;
            playPauseCheckbox.IsChecked = false;
        }

        int[] ListRandomPlay;
        bool Randomly = false;

        private void RandomPlay_Click(object sender, RoutedEventArgs e)
        {
            if (FullPaths.Count > 0)
            {
                Randomly = true;
                RanDomListSong();
            }
            else
            {
                checkBoxRandomPlay.IsChecked = false;
            }
        }

        void RanDomListSong()
        {
            int n = FullPaths.Count;
            ListRandomPlay = new int[n];

            //khời tạo mảng
            for (int i = 0; i < n; i++)
            {
                ListRandomPlay[i] = i;
            }

            //tiến hành random

            Random ran = new Random();
            for (int i = 0; i < 30; i++)
            {
                int a = ran.Next(0, n);
                int b = ran.Next(0, n);
                Swap(ref ListRandomPlay[a], ref ListRandomPlay[b]);
            }
        }
        void Swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }

        private void RemoveRanDomPlay_Click(object sender, RoutedEventArgs e)
        {
            Randomly = false;
        }

        bool isRepeatOne = false;
        private void RepeatOne_Checked(object sender, RoutedEventArgs e)
        {
            LoopRepeat.IsChecked = false;
            isLoopRepeat = false;
            isRepeatOne = !isRepeatOne;
        }

        bool isLoopRepeat = false;

        private void LoopRepeat_Click(object sender, RoutedEventArgs e)
        {
            RepeatOne.IsChecked = false;
            isRepeatOne = false;
            isLoopRepeat = !isLoopRepeat;
        }

        private void Slider_Click(object sender, MouseButtonEventArgs e)
        {
           
            var slider = sender as Slider;
            var testDuration = new TimeSpan(duration.Hours, duration.Minutes, (int)slider.Value);
            Player.Position = testDuration;

        }

        private void SaveToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            BindingList<FileInfo> playList = new BindingList<FileInfo>();

            foreach (FileInfo song in FullPaths)
            {
                playList.Add(song);
            }

            var screen = new NameDialog();
            string name;

            if (screen.ShowDialog() == true)
            {
                name = screen.name;
            }
            else
            {
                name = "Playlist";
            }

            dataFromFile.listOfPlayLists.Add(new PlaylistObject()
            {
                PlaylistName = name,
                Playlist = playList
            });

            ListBoxPlaylist.ItemsSource = dataFromFile.listOfPlayLists;
        }

        private void RemovePlaylist_Click(object sender, RoutedEventArgs e)
        {
            dataFromFile.listOfPlayLists.RemoveAt(ListBoxPlaylist.SelectedIndex);
        }

        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            using (StreamWriter sw = new StreamWriter(playListFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                string json = JsonConvert.SerializeObject(dataFromFile, Formatting.Indented);
                sw.WriteLine(json);
            }
        }

        private void LoadPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxPlaylist.SelectedIndex >= 0)
            {
                FullPaths = dataFromFile.listOfPlayLists[ListBoxPlaylist.SelectedIndex].Playlist;
                ListBoxFiles.ItemsSource = FullPaths;
            }
            else
            {
                MessageBox.Show("Select an playlist to load");
            }
        }

        List<FileInfo> ListRemove = new List<FileInfo>();
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;

            ListRemove.Add((FileInfo)checkbox.Tag);
            //MessageBox.Show(checkbox.Tag.ToString());
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            ListRemove.Remove((FileInfo)checkbox.Tag);
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            //int indexRemove = 0;
            for(int i=0;i<ListRemove.Count;i++)
            {
                FullPaths.Remove(ListRemove[i]);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            dataFromFile.lastTimePlaying = FullPaths;
            dataFromFile.index = currentIndex;

            using (StreamWriter sw = new StreamWriter(playListFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                string json = JsonConvert.SerializeObject(dataFromFile, Formatting.Indented);
                sw.WriteLine(json);
            }

            _hook.KeyUp -= KeyUp_hook;
            _hook.Dispose();
        }

        private void KeyUp_hook(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Control&& (e.KeyCode == System.Windows.Forms.Keys.Space))
            {
               if(isPlaying)
                {
                    Pause();
                }
               else
                {
                    Play();
                }
            }

            if (e.Control &&  (e.KeyCode == System.Windows.Forms.Keys.Right))
            {
                Next();
            }

            if (e.Control && (e.KeyCode == System.Windows.Forms.Keys.Left))
            {
                Previous();
            }
        }

        private void SelectSong_Click(object sender, MouseButtonEventArgs e)
        {
            Stop();
            playPauseCheckbox.IsChecked=true;
            currentIndex = ListBoxFiles.SelectedIndex;
            PlaySelectedIndex(currentIndex);
        }
    }
}
