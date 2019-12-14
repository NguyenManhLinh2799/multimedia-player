using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace multimedia_player
{
    class PlaylistObject
    {
        public string PlaylistName { get; set; }

        public BindingList<FileInfo> Playlist { get; set; }
    }
}
