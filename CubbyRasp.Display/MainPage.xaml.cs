using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.PlayTo;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CubbyRasp.Display
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly Random _random = new Random();

        private IReadOnlyList<StorageFile> _imageFiles;

        public MainPage()
        {
            InitializeComponent();

            MediaPlayer.TransportControls.IsNextTrackButtonVisible = true;
            MediaPlayer.TransportControls.IsPreviousTrackButtonVisible = true;
            MediaPlayer.TransportControls.IsFullWindowButtonVisible = false;
            MediaPlayer.TransportControls.IsZoomButtonVisible = false;



        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await LoadPictures();
            await LoadMusic();
        }

        private async Task LoadPictures()
        {
            var queryOption = new QueryOptions(CommonFileQuery.OrderByName, new[] { ".jpg" })
            {
                FolderDepth = FolderDepth.Deep
            };

            _imageFiles = await KnownFolders.RemovableDevices.CreateFileQueryWithOptions
                (queryOption).GetFilesAsync();

            using (var stream = await _imageFiles[_random.Next(0, _imageFiles.Count - 1)].OpenAsync(FileAccessMode.Read))
            {
                var bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(stream);
                BackgroundImage.Source = bitmapImage;
            }
        }

        private async Task LoadMusic()
        {
            var playbackList = new MediaPlaybackList
            {
                AutoRepeatEnabled = true
            };
            MediaPlayer.Source = playbackList;


            var queryOption = new QueryOptions(CommonFileQuery.OrderByName, new[] { ".mp3" })
            {
                FolderDepth = FolderDepth.Deep
            };

            foreach (var musicFile in await KnownFolders.RemovableDevices.CreateFileQueryWithOptions
                (queryOption).GetFilesAsync())
            {
                using (var stream = await musicFile.OpenAsync(FileAccessMode.Read))
                {
                    playbackList.Items.Add(new MediaPlaybackItem(MediaSource.CreateFromStream(stream, "audio/mpeg")));
                }
            }
        }

        private async void LeftImageButtonClicked(object sender, RoutedEventArgs e)
        {
            using (var stream = await _imageFiles[_random.Next(0, _imageFiles.Count - 1)].OpenAsync(FileAccessMode.Read))
            {
                var bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(stream);
                BackgroundImage.Source = bitmapImage;
            }
        }

        private async void RightImageButtonClicked(object sender, RoutedEventArgs e)
        {
            using (var stream = await _imageFiles[_random.Next(0, _imageFiles.Count - 1)].OpenAsync(FileAccessMode.Read))
            {
                var bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(stream);
                BackgroundImage.Source = bitmapImage;
            }
        }
    }



}
