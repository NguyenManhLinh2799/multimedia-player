using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                var filename = FullPaths[currentIndex].Name;
                //var converter = new NameConverter();
                //var shortname = converter.Convert(filename, null, null, null);

                var currentPos = Player.Position.ToString(@"mm\:ss");
                var duration = Player.NaturalDuration.TimeSpan.ToString(@"mm\:ss");

                currenttime.Content = $"{currentPos}/{duration}";
                
                int a = (Player.Position.Minutes * 60 + Player.Position.Seconds);

                
                //Slider.Maximum = a;
                //Slider.Value += 1;
                //Slider.
            }
            else
                Title = "No file selected...";
        }

        private void Player_MediaEnded(object sender, EventArgs e)
        {
            currentIndex++;
            PlaySelectedIndex(currentIndex);
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
                currentIndex = ListBoxFiles.SelectedIndex;
                PlaySelectedIndex(currentIndex);
                spin.Begin(disk);
            }
            else
            {
                MessageBox.Show("No file selected!");
                playPauseCheckbox.IsChecked = false;
            }
        
        }

        bool isPlaying=false;

        private void PlaySelectedIndex(int i)
        {
            string filename = FullPaths[i].FullName;

            Player.Open(new Uri(filename, UriKind.Absolute));

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
            }
            else
            {
                Player.Play();
                spin.Begin(disk);
            }
            isPlaying = !isPlaying;
        }
    }
}
