using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Pages;
using Mopups.Services;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.Xml.Linq;

namespace SongCollector
{
    public partial class MainViewModel : ObservableObject
    {
        //timeline
        [ObservableProperty]
        public ObservableCollection<Song> songs;
        [ObservableProperty]
        public MediaElement mediaElement;
        [ObservableProperty]
        public string pause;
        [ObservableProperty]
        private bool isPlayerVisible;
        private bool isPlaying;
        [ObservableProperty]
        private Song currentSong;
        private double oneSecond;
        [ObservableProperty]
        private double currentTime;
        [ObservableProperty]
        private double lineWidth;
        private readonly Database database;
        private FileResult file;
        
        public MainViewModel(Database database)
        {
            this.database = database;
            Pause = "&#xE802;";
            Loaded();
        }
        public async void Loaded()
        {
            Songs = new ObservableCollection<Song>(await database.GetAllSongs());
            IsPlayerVisible = false;
            MediaElement = new MediaElement()
            {
                ShouldAutoPlay = true,
                ShouldKeepScreenOn = true,
                IsVisible = false,
                HeightRequest = 0,
                WidthRequest = 0,
            };
            MediaElement.MediaEnded += MediaElement_MediaEnded;
            MediaElement.PositionChanged += MediaElement_PositionChanged;
            PopUp.NameAdded += Done;
        }

        private void MediaElement_MediaEnded(object sender, EventArgs e)
        {
            Next();
        }

        private void MediaElement_PositionChanged(object sender, CommunityToolkit.Maui.Core.Primitives.MediaPositionChangedEventArgs e)
        {
            CurrentTime += oneSecond;
        }
        [RelayCommand]
        private async void Tap(Song song)
        {
            isPlaying = true;
            MediaElement.Source = MediaSource.FromFile(song.Path);
            MediaElement.Play();
            IsPlayerVisible = true;
            CurrentSong = song;
            oneSecond = LineWidth / MediaElement.Duration.Seconds;
        }
        [RelayCommand]
        private async void Add()
        {
            file = await FilePicker.PickAsync();
            await Task.Delay(500);
            Show();
        }
        private void Show()
        {
            MopupService.Instance.PushAsync(new PopUp());

        }
        public async void Done(object e,NameAddEvent eventArgs)
        {
            var dbPath = DeviceInfo.Platform == DevicePlatform.iOS ?
                    Path.Combine(Environment
                  .GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", eventArgs.Name + ".mp3") :
                  Path.Combine(Environment
                  .GetFolderPath(Environment.SpecialFolder.ApplicationData), eventArgs.Name + ".mp3");
            using (var stream = file.OpenReadAsync())
            {
                using (FileStream outputFileStream = new FileStream(dbPath, FileMode.Create))
                {
                    stream.Result.CopyTo(outputFileStream);
                }
            }
            await database.SaveSong(new Song() { Name = eventArgs.Name, Path = dbPath });
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
                Song song = await database.GetSongById(CurrentSong.Id + 1);
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
            if (CurrentSong.Id != 1)
            {
                Song song = await database.GetSongById(CurrentSong.Id - 1);
                Tap(song);
            }
            else MediaElement.SeekTo(TimeSpan.Zero);
        }
    }
}

