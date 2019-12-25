using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace multimedia_player
{
    class Data
    {
        public BindingList<PlaylistObject> listOfPlayLists { get; set; }
        public BindingList<FileInfo> lastTimePlaying { get; set; }
    }
    class PlaylistObject: IValueConverter
    {
        public string PlaylistName { get; set; }

        public BindingList<FileInfo> Playlist { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
