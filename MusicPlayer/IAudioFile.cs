using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer
{
    public interface IAudioFile : IDisposable
    {
        public double Progress { get; }
        public TimeSpan Duration { get; }
        public bool IsPlaying { get; }
        public double Volume { get; set; }
        public void Play();
        public void Pause();
        public void Reset();
        public void Scrub(double portion);
    }
}
