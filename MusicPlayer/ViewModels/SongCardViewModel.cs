using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using MusicPlayer.Models;
using System.Windows.Input;


namespace MusicPlayer.ViewModels
{
    public partial class SongCardViewModel : ViewModelBase
    {
        private readonly SongData _data;
        private ICommand _onClicked;

        public SongCardViewModel(SongData data, ICommand onClicked)
        {
            _data = data;
            _icon = data.Metadata.Icon;
            _title = data.Metadata.Title;
            _artist = data.Metadata.Author;
            _onClicked = onClicked;
        }

        [ObservableProperty]
        private Bitmap _icon;
        [ObservableProperty]
        private string _title;
        [ObservableProperty]
        private string _artist;

        public SongData GetSongData() => _data;

        public void Command()
        {
            _onClicked.Execute(this);
        }
    }
}
