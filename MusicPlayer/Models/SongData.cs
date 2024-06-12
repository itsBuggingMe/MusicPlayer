using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Models
{
    public record class SongData(SongMetaData Metadata, IAudioFile Audio);
}
