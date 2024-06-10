using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;

namespace MusicPlayer.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public Bitmap? CurrentAlbumImage { get; } = new Bitmap(AssetLoader.Open(new Uri(
            "avares://"
            )));

        public MainViewModel()
        {
        }
    }
}
