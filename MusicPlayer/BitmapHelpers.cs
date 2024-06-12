using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer
{
    internal static class BitmapHelpers
    {
        public static Bitmap FromAssets(string name)
            => new Bitmap(AssetLoader.Open(new Uri($"avares://MusicPlayer/Assets/{name}")));
    }
}
