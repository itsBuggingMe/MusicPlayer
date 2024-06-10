using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Services
{
    public interface IMusicSource
    {
        void Save();
        void Load();
        IEnumerable<SongData> GetMusicFiles();
        Task<string> Download(Uri location, IProgress<double>? progress);
    }
}
