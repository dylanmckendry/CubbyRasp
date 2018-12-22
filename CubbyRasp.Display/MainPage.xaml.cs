using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
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

        private TrackedLinkedList<StorageFile> _imageFiles;

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
            LoadLights();
        }

        private void LoadLights()
        {
            GpioController gpio = GpioController.GetDefault();
            if (gpio == null)
            {
                return;
            }

            void SetUpToggleButton(ToggleButton toggleButton, int pinNumber)
            {
                var pin = gpio.OpenPin(pinNumber);
                pin.Write(GpioPinValue.High);
                pin.SetDriveMode(GpioPinDriveMode.Output);
                toggleButton.Checked += (sender, args) =>
                {
                    toggleButton.Content = "ON";
                    pin.Write(GpioPinValue.Low);
                };

                toggleButton.Unchecked += (sender, args) =>
                {
                    toggleButton.Content = "OFF";
                    pin.Write(GpioPinValue.High);
                };
            }

            SetUpToggleButton(this.AprilLight1Button, 2);
            SetUpToggleButton(this.AprilLight2Button, 3);
            SetUpToggleButton(this.EttaLight1Button, 19);
            SetUpToggleButton(this.EttaLight2Button, 26);
        }

        private async Task SetBackgroundPictureSource(StorageFile imageFile)
        {
            using (var stream = await imageFile.OpenAsync(FileAccessMode.Read))
            {
                var bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(stream);
                BackgroundImage.Source = bitmapImage;
            }
        }

        private async Task LoadPictures()
        {
            var queryOption = new QueryOptions(CommonFileQuery.OrderByName, new[] {".jpg"})
            {
                FolderDepth = FolderDepth.Deep
            };

            _imageFiles = new TrackedLinkedList<StorageFile>((await KnownFolders.RemovableDevices
                .CreateFileQueryWithOptions
                    (queryOption).GetFilesAsync()).ToList());

            await SetBackgroundPictureSource(_imageFiles.Next);
        }

        private async Task LoadMusic()
        {
            var playbackList = new MediaPlaybackList
            {
                AutoRepeatEnabled = true
            };
            MediaPlayer.Source = playbackList;


            var queryOption = new QueryOptions(CommonFileQuery.OrderByName, new[] {".mp3"})
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
            await SetBackgroundPictureSource(_imageFiles.Previous);

        }

        private async void RightImageButtonClicked(object sender, RoutedEventArgs e)
        {
            await SetBackgroundPictureSource(_imageFiles.Next);
        }

        private void BackgroundImageTapped(object sender, TappedRoutedEventArgs e)
        {
            ControlsGrid.Visibility = Visibility.Visible;
        }

        private void BackgroundGridDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ControlsGrid.Visibility = Visibility.Collapsed;
        }

        private bool _isSwiped;

        private async void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.IsInertial && !_isSwiped)
            {
                var swipedDistance = e.Cumulative.Translation.X;

                if (Math.Abs(swipedDistance) <= 2)
                {
                    return;
                }

                _isSwiped = true;

                if (swipedDistance > 0)
                {
                    await SetBackgroundPictureSource(_imageFiles.Next);
                }
                else
                {
                    await SetBackgroundPictureSource(_imageFiles.Previous);
                }
            }
        }

        private void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            _isSwiped = false;
        }

    }
}
