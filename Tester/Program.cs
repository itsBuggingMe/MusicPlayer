using MusicPlayer;
using MusicPlayer.Services;

namespace Tester
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            DesktopMusicSource source = new DesktopMusicSource();

            
            await source.Download(new Uri("https://www.youtube.com/watch?v=7M2Qt0RjktU"), new DelegateProgressReporter(Console.WriteLine));

            var t = source.GetMusicFiles().First();
            t.Audio.Play();

            while(true)
            {

            }
        }
    }
}
