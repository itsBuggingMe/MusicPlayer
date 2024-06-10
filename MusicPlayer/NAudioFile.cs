using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer
{
    public class NAudioFile : IAudioFile
    {
        public double Progress { get; private set; } = 0;

        public TimeSpan Duration { get; private set; }

        public bool IsPlaying { get; private set; } = false;

        private readonly AudioFileReader reader;
        private bool disposedValue;

        private static readonly WaveOutEvent waveOutEvent = new WaveOutEvent();
        private static AudioFileReader? CurrentlyLoaded;
        private static Action? FilePlayed;

        public NAudioFile(string path)
        {
            reader = new AudioFileReader(path);
            FilePlayed += OnFilePlayed;
        }

        public void Pause()
        {
            if(reader == CurrentlyLoaded)
            {
                waveOutEvent.Pause();
            }
        }

        public void Play()
        {
            lock (waveOutEvent)
            {
                if(CurrentlyLoaded != reader)
                {
                    waveOutEvent.Init(reader);
                    CurrentlyLoaded = reader;
                }

                waveOutEvent.Play();
                FilePlayed?.Invoke();
                IsPlaying = true;
            }
        }

        private void OnFilePlayed()
        {
            IsPlaying = false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (CurrentlyLoaded == reader)
                    {
                        waveOutEvent.Stop();
                        CurrentlyLoaded = null;
                    }
                    reader?.Dispose();
                    FilePlayed -= OnFilePlayed;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
