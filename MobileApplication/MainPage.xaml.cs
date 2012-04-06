﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using MetrocamPan.Models;
using Microsoft.Phone.Tasks;

using System.IO;
using ExifLib;

using System.Device;
using System.Device.Location;
using MobileClientLibrary.Models;
using MobileClientLibrary;
using JeffWilcox.FourthAndMayor;

namespace MetrocamPan
{
    public partial class MainPage : PhoneApplicationPage
    {
        public static String TwitterToken = null;
        public static String TwitterSecret = null;

        public static GeoCoordinateWatcher watcher;
        public static double lat = 0;
        public static double lng = 0;
        public static ObservableCollection<Picture> UserPictures = new ObservableCollection<Picture>();

        public static bool isFromLandingPage = false;
        public static bool isFromAppLaunch = false;
        public static bool isFromAppActivate = false;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Calls MainPage_Loaded when this page is constructed
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            setUpLocation();
        }

        // Set up location
        public static void setUpLocation()
        {
            if (watcher == null)
            {
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High); // using high accuracy
                watcher.MovementThreshold = 20; // use MovementThreshold to ignore noise in the signal
                watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
            }
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (popularHubTiles.ItemsSource == null)
            {
                GlobalLoading.Instance.IsLoading = true;
                Dispatcher.BeginInvoke(() =>
                    popularHubTiles.DataContext = App.PopularPictures);
            }
        }

        // When this page becomes active page in a frame
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Checks if user is already logged in previously
            if (!Settings.isLoggedIn.Value)
            {
                // Not logged in, navigate to landing page
                NavigationService.Navigate(new Uri("/LandingPage.xaml", UriKind.Relative));
            }

            // User is logged in previously, check how this app got to main page
            if (isFromAppLaunch)
            {
                // App is launched from start. We need to authenticate, then populate Popular, then populate Recent
                App.MetrocamService.AuthenticateCompleted += new RequestCompletedEventHandler(MetrocamService_AuthenticateCompleted);
                App.MetrocamService.Authenticate(Settings.username.Value, Settings.password.Value);
            }
            else if (isFromAppActivate)
            {
                // App is activated from tombstone. We need to authenticate, then populate Recent
                App.MetrocamService.AuthenticateCompleted += new RequestCompletedEventHandler(MetrocamService_AuthenticateCompleted);
                App.MetrocamService.Authenticate(Settings.username.Value, Settings.password.Value);
            }
            else if (isFromLandingPage)
            {
                // Reset back to false
                isFromLandingPage = false;

                // Clears back stack so user cannot go back to LandingPage(s)
                NavigationService.RemoveBackEntry();
                NavigationService.RemoveBackEntry();
                NavigationService.RemoveBackEntry();

                // App is from LandingPage (login or signup). We need to populate Popular, then populate Recent
                populatePopularPictures();
            }
            else
            {
                // MainPage is navigated here from a page in the forward stack, so do nothing
            }
        }

        void MetrocamService_AuthenticateCompleted(object sender, RequestCompletedEventArgs e)
        {
            // Unsubcribe
            App.MetrocamService.AuthenticateCompleted -= new RequestCompletedEventHandler(MetrocamService_AuthenticateCompleted);

            if (isFromAppLaunch)
            {
                populatePopularPictures();
            }
            if (isFromAppLaunch || isFromAppActivate)
            {
                fetchRecent();
            }

            // Reset back to false
            isFromAppLaunch = false;
            isFromAppActivate = false;
        }

        #region Popular Pivot Codebehind

        public static HubTile selectedPicture;
        private void popularPicture_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //HubTileService.FreezeGroup("PopularTiles");

            HubTile tile = sender as HubTile;
            PictureInfo info = tile.DataContext as PictureInfo;

            NavigationService.Navigate(new Uri("/PictureView.xaml?id=" + info.ID, UriKind.Relative));
        }

        public void populatePopularPictures()
        {
            App.MetrocamService.FetchPopularNewsFeedCompleted += new RequestCompletedEventHandler(MetrocamService_FetchPopularNewsFeedCompleted);
            App.MetrocamService.FetchPopularNewsFeed();
        }

        void MetrocamService_FetchPopularNewsFeedCompleted(object sender, MobileClientLibrary.RequestCompletedEventArgs e)
        {
            App.MetrocamService.FetchNewsFeedCompleted -= MetrocamService_FetchNewsFeedCompleted;
            App.PopularPictures.Clear();

            foreach (PictureInfo p in e.Data as List<PictureInfo>)
            {
                if (App.PopularPictures.Count == 24)
                    continue;

                // changes to local time
                p.FriendlyCreatedDate = TimeZoneInfo.ConvertTime(p.FriendlyCreatedDate, TimeZoneInfo.Local);

                App.PopularPictures.Add(p);
            }

            popularHubTiles.ItemsSource = App.PopularPictures;
        }

        #endregion Popular Pivot Codebehind

        #region News Feed Codebehind

        private void fetchRecent()
        {
            App.MetrocamService.FetchNewsFeedCompleted += new RequestCompletedEventHandler(MetrocamService_FetchNewsFeedCompleted);
            App.MetrocamService.FetchNewsFeed();
        }

        void MetrocamService_FetchNewsFeedCompleted(object sender, RequestCompletedEventArgs e)
        {
            App.MetrocamService.FetchNewsFeedCompleted -= MetrocamService_FetchNewsFeedCompleted;
            App.RecentPictures.Clear();

            foreach (PictureInfo p in e.Data as List<PictureInfo>)
            {
                if (App.RecentPictures.Count == 10)
                    break;

                // changes to local time
                p.FriendlyCreatedDate = TimeZoneInfo.ConvertTime(p.FriendlyCreatedDate, TimeZoneInfo.Local);

                App.RecentPictures.Add(p);
            }
        }

        private void recentPicture_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image image = sender as Image;
            PictureInfo info = image.DataContext as PictureInfo;

            NavigationService.Navigate(new Uri("/PictureView.xaml?id=" + info.ID, UriKind.Relative));
        }

        private void ViewUserDetail_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image image = sender as Image;
            PictureInfo info = image.DataContext as PictureInfo;

            NavigationService.Navigate(new Uri("/UserDetailPage.xaml?id=" + info.User.ID, UriKind.Relative));
        }

        private void ViewUserDetailFromUsername_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TextBlock username = sender as TextBlock;
            PictureInfo info = username.DataContext as PictureInfo;

            NavigationService.Navigate(new Uri("/UserDetailPage.xaml?id=" + info.User.ID, UriKind.Relative));
        }

        #endregion News Feed Codebehind

        #region Application Bar Codebehind

        #region Camera Button

        private void CameraButton_Click(object sender, EventArgs e)
        {
            CameraCaptureTask cam = new CameraCaptureTask();
            cam.Completed += new EventHandler<PhotoResult>(cameraCaptureTask_Completed);

            cam.Show();
        }

        public static Boolean isLandscape = false;
        public static Image captured = new Image();
        public static BitmapImage bmp = new BitmapImage();
        public static Boolean tookPhoto = false;
        private void cameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            // if no picture was taken
            if (e.ChosenPhoto == null)
                return;

            // collect location data
            watcher.Start();

            tookPhoto = true;

            bool land = false;
            // figure out the orientation from EXIF data

            e.ChosenPhoto.Position = 0;
            JpegInfo info = ExifReader.ReadJpeg(e.ChosenPhoto, e.OriginalFileName);

            int _width = info.Width;
            int _height = info.Height;
            var _orientation = info.Orientation;
            int _angle = 0;

            switch (info.Orientation)
            {
                case ExifOrientation.TopLeft:
                case ExifOrientation.Undefined:
                    _angle = 0;
                    land = true;
                    break;
                case ExifOrientation.TopRight:
                    _angle = 90;
                    break;
                case ExifOrientation.BottomRight:
                    _angle = 180;
                    land = true;
                    break;
                case ExifOrientation.BottomLeft:
                    _angle = 270;
                    break;
            }

            if (_angle > 0d)
            {
                bmp.SetSource(RotateStream(e.ChosenPhoto, _angle));
            }
            else
            {
                bmp.SetSource(e.ChosenPhoto);
            }

            captured.Source = bmp;

            // wait til UI thread is done, then navigate
            Dispatcher.BeginInvoke(() =>
            {
                if (land)
                {
                    NavigationService.Navigate(new Uri("/CropPageLandscapeOrientation.xaml", UriKind.Relative));
                }
                else
                {
                    NavigationService.Navigate(new Uri("/CropPage.xaml", UriKind.Relative));
                }
            });
        }

        /**
         * rotates picture appropriately
         */
        private Stream RotateStream(Stream stream, int angle)
        {
            stream.Position = 0;
            if (angle % 90 != 0 || angle < 0) throw new ArgumentException();
            if (angle % 360 == 0) return stream;

            BitmapImage bitmap = new BitmapImage();
            bitmap.SetSource(stream);
            WriteableBitmap wbSource = new WriteableBitmap(bitmap);

            WriteableBitmap wbTarget = null;
            if (angle % 180 == 0)
            {
                wbTarget = new WriteableBitmap(wbSource.PixelWidth, wbSource.PixelHeight);
            }
            else
            {
                wbTarget = new WriteableBitmap(wbSource.PixelHeight, wbSource.PixelWidth);
            }

            for (int x = 0; x < wbSource.PixelWidth; x++)
            {
                for (int y = 0; y < wbSource.PixelHeight; y++)
                {
                    switch (angle % 360)
                    {
                        case 90:
                            wbTarget.Pixels[(wbSource.PixelHeight - y - 1) + x * wbTarget.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                            break;
                        case 180:
                            wbTarget.Pixels[(wbSource.PixelWidth - x - 1) + (wbSource.PixelHeight - y - 1) * wbSource.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                            break;
                        case 270:
                            wbTarget.Pixels[y + (wbSource.PixelWidth - x - 1) * wbTarget.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                            break;
                    }
                }
            }
            MemoryStream targetStream = new MemoryStream();
            wbTarget.SaveJpeg(targetStream, wbTarget.PixelWidth, wbTarget.PixelHeight, 0, 100);
            return targetStream;
        }

        // Sign out
        private void SignoutBarIconButton_Click(object sender, EventArgs e)
        {
            // Reset all isolate storage Setting objects to default values
            Settings.logoutUser();

            // Navigate to landing page
            NavigationService.Navigate(new Uri("/LandingPage.xaml", UriKind.Relative));
        }
        #endregion

        #region ChoosePicture Button
        private void ChoosePicture_Click(object sender, EventArgs e)
        {
            PhotoChooserTask picker = new PhotoChooserTask();
            picker.ShowCamera = false;
            picker.Completed += new EventHandler<PhotoResult>(picker_Completed);

            picker.Show();
        }

        public static int MAX_HEIGHT = 92;
        public static int MAX_WIDTH = 110;
        void picker_Completed(object sender, PhotoResult e)
        {
            tookPhoto = false;

            if (e.ChosenPhoto == null)
                return;

            e.ChosenPhoto.Position = 0;
            JpegInfo info = ExifReader.ReadJpeg(e.ChosenPhoto, e.OriginalFileName);

            /**
             * get image location
             */
            lat = info.GpsLatitude[0];
            lng = info.GpsLongitude[0];

            if (info.GpsLatitudeRef == ExifGpsLatitudeRef.South)
                lat = lat * -1;

            if (info.GpsLongitudeRef == ExifGpsLongitudeRef.West)
                lng = lng * -1;
            ///////////////////////

            bmp.SetSource(e.ChosenPhoto);
            captured.Source = bmp;

            Dispatcher.BeginInvoke(() =>
            {
                /**
                 * determine if picture is Portrait or Landscape
                 */
                if ((Convert.ToDouble(bmp.PixelHeight) / Convert.ToDouble(bmp.PixelWidth)) < 1)
                {
                    NavigationService.Navigate(new Uri("/CropPageLandscapeOrientation.xaml", UriKind.Relative));
                }
                else
                {
                    NavigationService.Navigate(new Uri("/CropPage.xaml", UriKind.Relative));
                }
            });
        }

        #endregion

        #region Menu Options

        private void Settings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        private void UserSearch_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/UserSearch.xaml", UriKind.Relative));
        }

        private void EditProfile_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/EditProfile.xaml", UriKind.Relative));
        }

        private void About_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        #endregion

        #endregion Application Bar Codebehind

        // Event handler for the GeoCoordinateWatcher.StatusChanged event.
        public static Boolean hasLocationData = false;
        public static void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    // The Location Service is disabled or unsupported.
                    // Check to see whether the user has disabled the Location Service.
                    if (watcher.Permission == GeoPositionPermission.Denied)
                    {
                        // The user has disabled the Location Service on their device.
                        hasLocationData = false;
                    }
                    else
                    {
                        hasLocationData = false;
                    }
                    break;

                case GeoPositionStatus.NoData:
                    // The Location Service is working, but it cannot get location data.
                    // Alert the user and enable the Stop Location button.
                    hasLocationData = false;
                    break;

                case GeoPositionStatus.Ready:
                    // The Location Service is working and is receiving location data.
                    // Show the current position and enable the Stop Location button.
                    lat = watcher.Position.Location.Latitude;
                    lng = watcher.Position.Location.Longitude;
                    hasLocationData = true;
                    watcher.Stop();
                    break;
            }
        }

        private void HubTile_Loaded(object sender, RoutedEventArgs e)
        {
            if (LoadingMessage.Visibility == Visibility.Visible)
            {
                GlobalLoading.Instance.IsLoading = false;
                LoadingMessage.Visibility = Visibility.Collapsed;
            }

            HubTile curr = sender as HubTile;
            curr.Visibility = Visibility.Visible;
        }

        private void MainContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainContent.SelectedIndex == 1 && recentPictures.ItemsSource == null)
                Dispatcher.BeginInvoke(() =>
                {
                    GlobalLoading.Instance.IsLoading = true;
                    recentPictures.DataContext = App.RecentPictures;
                    GlobalLoading.Instance.IsLoading = false;
                });
        }
    }
}