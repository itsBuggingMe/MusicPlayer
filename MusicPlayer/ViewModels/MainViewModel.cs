using Avalonia.Controls.Documents;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DialogHostAvalonia;
using MusicPlayer.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicPlayer.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private Bitmap _currentAlbumCover = BitmapHelpers.FromAssets("DefaultCover.png");

        [ObservableProperty]
        private bool _isDialogOpen;

        [ObservableProperty]
        private string? _youtubeUrl;
        public ObservableCollection<SongCardViewModel> Cards { get; init; }

        private ICommand _leftItemClick;

        private IMusicSource _musicSource;

        //Is this dependency injection?
        public MainViewModel(IMusicSource musicSource)
        {
            _musicSource = musicSource;
            musicSource.Load();
            _leftItemClick = new RelayCommand<SongCardViewModel>(MusicCardClicked);
            Cards = new(_musicSource.GetMusicFiles().Select(f => new SongCardViewModel(f, _leftItemClick)));
        }

        private void MusicCardClicked(SongCardViewModel? songCardViewModel)
        {
            if(songCardViewModel is not null)
            {
                CurrentAlbumCover = songCardViewModel.Icon;
                songCardViewModel.GetSongData().Audio.Play();
            }
        }

        [RelayCommand]
        public void AddSongCommand() => IsDialogOpen = true;

        [RelayCommand]
        public async Task<object?> ExitDialog()
        {
            IsDialogOpen = false;
            if(!string.IsNullOrEmpty(YoutubeUrl))
            {
                object o = await _musicSource.Download(new Uri(YoutubeUrl), new DelegateProgressReporter(d => Debug.WriteLine(d)));
                _musicSource.Load();

                foreach (var file in _musicSource.GetMusicFiles())
                {
                    if (!Cards.Any(vm => vm.GetSongData().Metadata.VideoID == file.Metadata.VideoID))
                    {
                        Cards.Add(new SongCardViewModel(file, _leftItemClick));
                    }
                }
                //tmp so i can listen while i work
                Cards.LastOrDefault()?.GetSongData().Audio.Play();
                return o;
            }

            YoutubeUrl = null;
            return null;
        }
    }
}
