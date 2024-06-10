using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos;
using YoutubeExplode.Common;
using System.Net.Http;
using TagFile = TagLib.File;

namespace MusicPlayer.Services
{
    public class DesktopMusicSource : IMusicSource//TODO: Implement IDisposable
    {
        private static string SubPath => "DesktopData";
        private YoutubeClient _client;
        private HttpClient _http;
        private readonly Task[] _taskArr = new Task[2];

        private List<SongData> _songCache = new();

        public DesktopMusicSource()
        {
            _client = new();
            _http = new();
        }

        public IEnumerable<SongData> GetMusicFiles()
        {
            foreach(string path in EnumerateFiles())
            {
                if (path.EndsWith(".mp3"))
                {
                    TagFile f = TagFile.Create(path);

                    string s = new string(Path.GetFileName(path.AsSpan())[..^4]);
                    SongMetaData metaData = new SongMetaData(
                        f.Tag.Title, 
                        f.Tag.AlbumArtists.FirstOrDefault() ?? string.Empty,
                        TimeSpan.Parse(f.Tag.Length),
                        s);

                    yield return new SongData(metaData, new NAudioFile(path));
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
            string path = Path.Combine(SubPath, $"{id}.mp3");
            string thPath = Path.Combine(SubPath, $"{id}.png");

            if (EnumerateFiles().Any(p => p == path))
            {
                return path;
            }

            Task<(Video, HttpResponseMessage)> thumbnail;

            _taskArr[0] = thumbnail = _client.Videos.GetAsync(id).AsTask()
                //get metadata & download
                .ContinueWith(async t => {
                    var tmp = await t;
                    return (tmp, await _http.GetAsync(tmp.Thumbnails.GetWithHighestResolution().Url));
                    }).Unwrap();

            _taskArr[1] = _client.Videos.DownloadAsync(id, path, o => o
                    .SetContainer("mp3")
                    .SetPreset(ConversionPreset.UltraFast)
                    .SetFFmpegPath("ffmpeg/ffmpeg.exe"),
                    reporter).AsTask();

            await Task.WhenAll(_taskArr);

            var msg = await thumbnail;
            Stream s = msg.Item2.Content.ReadAsStream();

            using (var fileStream = File.Create(thPath))
            {
                s.Seek(0, SeekOrigin.Begin);
                s.CopyTo(fileStream);
            }
            GetArtistTitle(msg.Item1.Title, out var a, out var t);

            TagFile f = TagFile.Create(path);
            f.Tag.Title = t;
            f.Tag.AlbumArtists = [a];
            f.Tag.Length = msg.Item1.Duration.ToString();
            f.Save();

            return path;
        }

        private IEnumerable<string> EnumerateFiles()
        {
            if (CreateIfNotExist())
            {
                return Directory.EnumerateFiles(SubPath);
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
            const string Unknown = "Unknown";
            string[] sections = input.Split('-');
            author = Unknown;
            title = Unknown;

            if (sections.Length > 0)
            {
                author = sections[0].Trim();
            }

            if(sections.Length > 1)
            {
                title = sections[1].Trim();
            }
        }
    }
}
