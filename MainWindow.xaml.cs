using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

        public MainWindow()
        {
            InitializeComponent();
            Player.MediaEnded += Player_MediaEnded;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (Player.Source != null)
            {
                //var filename = FullPaths[currentIndex].Name;
                //var converter = new NameConverter();
                //var shortname = converter.Convert(filename, null, null, null);
                var converter = new NameConverter();
                var currentPos = Player.Position.ToString(@"mm\:ss");
                

                currenttime.Content = $"{currentPos}/{duration.Minutes}:{duration.Seconds}-{currentIndex}";
                
                int timeOfPlayer = Player.NaturalDuration.TimeSpan.Minutes * 60 + Player.NaturalDuration.TimeSpan.Seconds;
                Slider.Maximum = timeOfPlayer;
                Slider.Value += 1;

                currenttime.Content = $"{currentPos}/{duration}-{currentIndex}";
                
                int a = (Player.Position.Minutes * 60 + Player.Position.Seconds);

             
                //Slider.Maximum = a;
                Slider.Value += 1;
                //Slider.
            }
            else
                Title = "No file selected...";
        }

        int currentRandomPlay = 0;

        private void Player_MediaEnded(object sender, EventArgs e)
        {

            if (currentIndex < FullPaths.Count - 1)
            {
                currentIndex++;
            }

            if(Randomly)
            {
                if(currentRandomPlay < RandomPlay.Length - 1)
                {
                    currentRandomPlay++;
                    currentIndex = RandomPlay[currentRandomPlay];
                }
            }
            PlaySelectedIndex(currentIndex);
            Slider.Value = 0;
        }

        BindingList<FileInfo> FullPaths = new BindingList<FileInfo>();

        private void addFile_Click(object sender, RoutedEventArgs e)
        {
            var screen = new OpenFileDialog();
            if(screen.ShowDialog() == true)
            {
                var info = new FileInfo(screen.FileName);
                FullPaths.Add(info);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ListBoxFiles.ItemsSource = FullPaths;
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            Storyboard spin = (Storyboard)FindResource("startSpin");

            if (ListBoxFiles.SelectedIndex >= 0)
            {
                if (isPausing == true)
                {
                    Player.Play();
                    isPlaying = true;
                }
                else
                {
                    if (Randomly)
                    {
                        currentIndex = RandomPlay[currentRandomPlay];
                    }
                    else
                    {
                        currentIndex = ListBoxFiles.SelectedIndex;
                    }
                }

                PlaySelectedIndex(currentIndex);

            }
            else
            {
                MessageBox.Show("No file selected!");
                playPauseCheckbox.IsChecked = false;
            }

            //if (isPausing)
            //{
            //    isPlaying = true;
            //    Player.Play();
            //}
            //else
            //{


            //    if (Randomly)
            //    {
            //        currentIndex = RandomPlay[currentRandomPlay];
            //    }
            //    else
            //    {
            //        if (ListBoxFiles.SelectedIndex >= 0)
            //        {
            //            currentIndex = ListBoxFiles.SelectedIndex;
            //        }
            //        else
            //        {
            //            MessageBox.Show("No file selected!");
            //            playPauseCheckbox.IsChecked = false;
            //        }
            //    }
            //}

        }

        bool isPlaying=false;
        bool isPausing = false;

        private void PlaySelectedIndex(int i)
        {
            string filename = FullPaths[i].FullName;
            Debug.WriteLine(FullPaths[i]);

            Player.Open(new Uri(filename, UriKind.Absolute));

            duration = Player.NaturalDuration.TimeSpan;
            Player.Play();
            isPlaying = true;
            timer.Start();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            Storyboard spin = (Storyboard)FindResource("startSpin");

            if (isPlaying)
            {
                Player.Pause();
                spin.Stop(disk);
                isPausing = true;
                timer.Stop();
            }
            isPlaying = !isPlaying;
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (currentIndex > 0)
            {
                Player.Stop();
                currentIndex -= 1;
                PlaySelectedIndex(currentIndex);
                //ListBoxFiles.Items.CurrentItem(currentIndex)
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (currentIndex < ListBoxFiles.Items.Count - 1)
            {
                Player.Stop();
                currentIndex += 1;
                PlaySelectedIndex(currentIndex);
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
            {
                Player.Stop();
                timer.Stop();
                isPausing = false;
                isPlaying = false;
                Slider.Value = 0;
                playPauseCheckbox.IsChecked = false;
            }
        }

        int[] RandomPlay;
        bool Randomly = false;
        private void RandomPlay_Click(object sender, RoutedEventArgs e)
        {
            Randomly = true;
            int n = FullPaths.Count;
            RandomPlay = new int[n];

            //khời tạo mảng
            for (int i = 0; i < n; i++)
            {
                RandomPlay[i] = i;
            }

            //tiến hành random

            Random ran = new Random();
            for (int i = 0; i < 30; i++)
            {
                int a = ran.Next(0, n);
                int b = ran.Next(0, n);
                Swap(ref RandomPlay[a], ref RandomPlay[b]);
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
    }
}
