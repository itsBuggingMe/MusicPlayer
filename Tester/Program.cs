using MusicPlayer;
using MusicPlayer.Services;

namespace Tester
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            DesktopMusicSource source = new DesktopMusicSource();

            Console.WriteLine("Enter youtube link:");
            await source.Download(
                new Uri(Console.ReadLine() ?? "https://www.youtube.com/watch?v=7M2Qt0RjktU"), 
                new DelegateProgressReporter(Console.WriteLine));

            var t = source.GetMusicFiles().First();
            t.Audio.Play();
            Console.WriteLine("Now Playing " + t);
            while(true)
            {
                if(!t.Audio.IsPlaying)
                {
                    t.Audio.Reset();
                    t.Audio.Play();
                }
            }
        }
    }
}
