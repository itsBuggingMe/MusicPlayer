using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Models
{
    public class NAudioFile : IAudioFile
    {
        public double Progress { get; private set; } = 0;

        public TimeSpan Duration { get; private set; }

        public bool IsPlaying { get; private set; } = false;

        public double Volume { get => waveOutEvent.Volume; set => waveOutEvent.Volume = (float)value; }

        private readonly AudioFileReader reader;
        private bool disposedValue;

        private static readonly WaveOutEvent waveOutEvent = new WaveOutEvent();
        private static AudioFileReader? CurrentlyLoaded;
        private static Action? FilePlayed;

        public NAudioFile(string path)
        {
            reader = new AudioFileReader(path);
            FilePlayed += OnFilePlayed;
            waveOutEvent.PlaybackStopped += WaveOutEvent_PlaybackStopped;
        }

        private void WaveOutEvent_PlaybackStopped(object? sender, StoppedEventArgs e)
            => IsPlaying = false;
        private void OnFilePlayed()
            => IsPlaying = false;

        public void Pause()
        {
            if (reader == CurrentlyLoaded)
                waveOutEvent.Pause();
        }

        public void Reset()
        {
            if (reader == CurrentlyLoaded)
                waveOutEvent.Stop();
        }

        public void Scrub(double portion)
        {
            throw new NotImplementedException();
        }

        public void Play()
        {
            lock (waveOutEvent)
            {
                if (CurrentlyLoaded != reader)
                {
                    waveOutEvent.Stop();
                    waveOutEvent.Init(reader);
                    CurrentlyLoaded = reader;
                }

                waveOutEvent.Play();
                FilePlayed?.Invoke();
                IsPlaying = true;
            }
        }

        #region Disposing
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
                    waveOutEvent.PlaybackStopped -= WaveOutEvent_PlaybackStopped;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
