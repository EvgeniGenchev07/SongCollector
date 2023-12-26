using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;

namespace SongCollector
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<Song> songs;
        [ObservableProperty]
        public MediaElement mediaElement;
        [ObservableProperty]
        private string name;
        [ObservableProperty]
        private bool isPlayerVisible;
        private bool isPlaying;
        private int currentSongId;
        private readonly Database database;
        public MainViewModel(Database database)
        {
            this.database = database;
            Loaded();
        }
        public async void Loaded()
        {
            Songs = new ObservableCollection<Song>(await database.GetAllSongs());
            isPlayerVisible = false;
            MediaElement = new MediaElement()
            {
                ShouldAutoPlay = true,
                ShouldKeepScreenOn = true,
                IsVisible = false,
                HeightRequest = 0,
                WidthRequest = 0,
            };
        //    MediaElement.MediaEnded += Next1();
        }


        [RelayCommand]
        private async void Tap(Song song)
        {
            isPlaying = true;
            MediaElement.Source = MediaSource.FromFile(song.Path);
            MediaElement.Play();
            IsPlayerVisible = true;
            currentSongId = song.Id;
        }
        [RelayCommand]
        private async void Add()
        {
            var pick = await FilePicker.PickAsync();
            var dbPath = DeviceInfo.Platform == DevicePlatform.iOS ?
                    Path.Combine(Environment
                  .GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", Name + ".mp3") :
                  Path.Combine(Environment
                  .GetFolderPath(Environment.SpecialFolder.ApplicationData), Name + ".mp3");
            using (var stream = pick.OpenReadAsync())
            {
                using (FileStream outputFileStream = new FileStream(dbPath, FileMode.Create))
                {
                    stream.Result.CopyTo(outputFileStream);
                }
            }
            await database.SaveSong(new Song() { Name = Name, Path = dbPath });
            Loaded();
        }
        [RelayCommand]
        public void PauseAndPlay()
        {
            if (isPlaying)
            {
                MediaElement.Pause();
                isPlaying = false;
            }
            else
            {
                MediaElement.Play();
                isPlaying = true;
            }
        }
        [RelayCommand]
        public async void Next()
        {
            try
            {
                Song song = await database.GetSongById(currentSongId + 1);
                if (song != null) Tap(song);
                else MediaElement.SeekTo(TimeSpan.Zero);
            }
            catch (Exception ex)
            {
                MediaElement.SeekTo(TimeSpan.Zero);
            }
        }
        [RelayCommand]
        public async void Previous()
        {
            if (currentSongId != 1)
            {
                Song song = await database.GetSongById(currentSongId - 1);
                Tap(song);
            }
            else MediaElement.SeekTo(TimeSpan.Zero);
        }
    }
}

