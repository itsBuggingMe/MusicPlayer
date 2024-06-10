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
        public void Play();
        public void Pause();
    }
}
