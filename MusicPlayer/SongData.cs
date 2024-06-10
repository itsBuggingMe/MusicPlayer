using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer
{
    public record class SongData(SongMetaData Metadata, IAudioFile Audio);
    public record class SongMetaData(string Title, string Author, TimeSpan Length, string VideoID);
}
