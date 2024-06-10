using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos;
using YoutubeExplode.Common;
using System.Net.Http;
using ReactiveUI;
using Avalonia.Media.Imaging;

namespace MusicPlayer.Services
{
    public class DesktopMusicSource : IMusicSource//TODO: Implement IDisposable
    {
        private static string SubPath => "data";
        private YoutubeClient _client;
        private HttpClient _http;
        private readonly Task[] _taskArr = new Task[2];

        public DesktopMusicSource()
        {
            _client = new();
            _http = new();
        }

        public IEnumerable<SongData> GetMusicFiles()
        {
            foreach(string dir in EnumerateDirs())
            {
                foreach(string path in Directory.EnumerateFiles(dir))
                {
                    if (path.EndsWith(".mp3"))
                    {
                        ReadOnlySpan<char> p = Path.GetFileName(dir.AsSpan())[..^4];

                        string s = new string(p);
                        yield return new SongData(new SongMetaData(), new NAudioFile(path));
                    }
                }
            }
        }

        public void Load()
        {

        }

        public void Save()
        {

        }

        public async Task<string> Download(Uri location, IProgress<double>? reporter = null)
        {
            CreateIfNotExist();

            VideoId id = VideoId.Parse(location.AbsoluteUri);
            string path = Path.Combine(SubPath, id, "audio.mp3");
            string thPath = Path.Combine(SubPath, id, "img.png");
            string dir = Path.Combine(SubPath, id);
            string meta = Path.Combine(SubPath, id);

            if (EnumerateDirs().Any(p => p.Contains(id.Value)))
            {
                return path;
            }

            Directory.CreateDirectory(dir);

            Task<HttpResponseMessage> thumbnail;
            _taskArr[0] = thumbnail = _client.Videos.GetAsync(id).AsTask()
                //get metadata & download
                .ContinueWith(async t => await _http.GetAsync((await t).Thumbnails.GetWithHighestResolution().Url)).Unwrap();

            _taskArr[1] = _client.Videos.DownloadAsync(id, path, o => o
                    .SetContainer("mp3")
                    .SetPreset(ConversionPreset.UltraFast)
                    .SetFFmpegPath("ffmpeg/ffmpeg.exe"),
                    reporter).AsTask();

            await Task.WhenAll(_taskArr);

            HttpResponseMessage msg = await thumbnail;
            Stream s = msg.Content.ReadAsStream();

            using (var fileStream = File.Create(thPath))
            {
                s.Seek(0, SeekOrigin.Begin);
                s.CopyTo(fileStream);
            }

            File.WriteAllText();

            return path;
        }

        private IEnumerable<string> EnumerateDirs()
        {
            if (CreateIfNotExist())
            {
                return Directory.EnumerateDirectories(SubPath);
            }

            return [];
        }

        private bool CreateIfNotExist()
        {
            if (!Directory.Exists(SubPath))
            {
                Directory.CreateDirectory(SubPath);
                return false;
            }
            return true;
        }

        //Here, we just assume its in the format of Author - Title
        private void GetArtistTitle(string input, out string author, out string title)
        {

        }
    }
}
