using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Models
{
    public record class SongMetaData(string Title, string Author, TimeSpan Length, string VideoID, Bitmap Icon);
}
