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
using Microsoft.Phone.Tasks;

using MetrocamPan.Models;

using ExifLib;
using System.Windows.Media.Imaging;
using System.IO;
using MobileClientLibrary.Models;
using MobileClientLibrary;
using MetrocamPan.Helpers;
using Microsoft.Phone.Shell;
using JeffWilcox.FourthAndMayor;
using System.Windows.Navigation;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Media;
using Coding4Fun.Phone.Controls;

namespace MetrocamPan
{
    public partial class PictureView : PhoneApplicationPage
    {
        PictureInfo CurrentPicture = null;
        Boolean alreadyAddedButton = false;
        Boolean alreadyAddedMenuItem = false;
        Boolean doingWork = false;

        private ToastPrompt toastDisplay;

        public PictureView()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationContext.QueryString["type"].Equals("popular"))
            {
                CurrentPicture = (from pic in App.PopularPictures where pic.ID.Equals(NavigationContext.QueryString["id"]) select pic).First<PictureInfo>();
            }
            else if (NavigationContext.QueryString["type"].Equals("recent"))
            {
                CurrentPicture = (from pic in App.RecentPictures where pic.ID.Equals(NavigationContext.QueryString["id"]) select pic).First<PictureInfo>();
            }
            else if (NavigationContext.QueryString["type"].Equals("user"))
            {
                CurrentPicture = (from pic in App.UserPictures where pic.ID.Equals(NavigationContext.QueryString["id"]) select pic).First<PictureInfo>();
            }
            else if (NavigationContext.QueryString["type"].Equals("favorite"))
            {
                CurrentPicture = (from pic in App.FavoritedUserPictures where pic.ID.Equals(NavigationContext.QueryString["id"]) select pic).First<PictureInfo>();
            }

            if (CurrentPicture.User.ProfilePicture != null)
            {
                BitmapImage b = new BitmapImage(new Uri(CurrentPicture.User.ProfilePicture.MediumURL, UriKind.RelativeOrAbsolute));
                this.imgProfilePicture.Source = b;
            }

            this.imgPicture.Source = new BitmapImage(new Uri(CurrentPicture.MediumURL));
            this.txtUsername.Text = CurrentPicture.User.Username;
            this.txtCaption.Text = CurrentPicture.Caption;
            this.txtCreatedDate.Text = FriendlierTime.Convert(CurrentPicture.FriendlyCreatedDate);

            if (!alreadyAddedButton)
            {
                if (!CurrentPicture.IsFavorited)
                {
                    ApplicationBarIconButton favorite = new ApplicationBarIconButton(new Uri("Images/appbar.heart.png", UriKind.Relative));
                    favorite.Text = "favorite";
                    favorite.Click += new EventHandler(Favorite_Click);

                    ApplicationBar.Buttons.Add(favorite);

                    App.MetrocamService.FetchFavoritedPictureByPictureIDCompleted +=new RequestCompletedEventHandler(MetrocamService_FetchFavoritedPictureByPictureIDCompleted);
                    App.MetrocamService.FetchFavoritedPictureByPictureID(CurrentPicture.ID, App.MetrocamService.CurrentUser.ID);
                }
                else
                {
                    ApplicationBarIconButton unfavorite = new ApplicationBarIconButton(new Uri("Images/appbar.heartbreak.png", UriKind.Relative));
                    unfavorite.Text = "unfavorite";
                    unfavorite.Click += new EventHandler(Unfavorite_Click);

                    ApplicationBar.Buttons.Add(unfavorite);
                }
            }

            if (CurrentPicture.User.ID.Equals(App.MetrocamService.CurrentUser.ID))
            {
                if (!alreadyAddedMenuItem)
                {
                    ApplicationBarMenuItem profilePic = new ApplicationBarMenuItem();
                    profilePic.Text = "make profile picture";
                    profilePic.Click += new EventHandler(MakeProfilePicture);

                    ApplicationBarMenuItem Delete = new ApplicationBarMenuItem();
                    Delete.Text = "delete";
                    Delete.Click += new EventHandler(Delete_Click);

                    ApplicationBarMenuItem Save = new ApplicationBarMenuItem();
                    Save.Text = "save";
                    Save.Click += new EventHandler(Save_Click);

                    ApplicationBar.MenuItems.Add(Delete);
                    ApplicationBar.MenuItems.Add(Save);
                    ApplicationBar.MenuItems.Add(profilePic);

                    alreadyAddedMenuItem = true;
                }
            }
            else
            {
                if (!alreadyAddedMenuItem)
                {
                    ApplicationBarMenuItem Save = new ApplicationBarMenuItem();
                    Save.Text = "save";
                    Save.Click += new EventHandler(Save_Click);

                    ApplicationBar.MenuItems.Add(Save);
                    alreadyAddedMenuItem = true;
                }
            }
        }

        FavoritedPicture f = null;
        void MetrocamService_FetchFavoritedPictureByPictureIDCompleted(object sender, RequestCompletedEventArgs e)
        {
            App.MetrocamService.FetchFavoritedPictureByPictureIDCompleted -= MetrocamService_FetchFavoritedPictureByPictureIDCompleted;
            f = e.Data as FavoritedPicture;
        }

        void Delete_Click(object sender, EventArgs e)
        {
            if (CurrentPicture == null)
                return;

            App.MetrocamService.DeletePictureCompleted += new RequestCompletedEventHandler(MetrocamService_DeletePictureCompleted);
            GlobalLoading.Instance.IsLoading = true;
            App.MetrocamService.DeletePicture(CurrentPicture.ID);
        }

        void MetrocamService_DeletePictureCompleted(object sender, RequestCompletedEventArgs e)
        {
            GlobalLoading.Instance.IsLoading = false;

            App.pictureIsDeleted = true;

            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        void Save_Click(object sender, EventArgs e)
        {
            PictureInfo info = CurrentPicture;

            String file = info.User.Username + info.ID + ".jpg";

            var myStore = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream myFileStream = myStore.CreateFile(file);

            WriteableBitmap bitmap = new WriteableBitmap((BitmapSource)this.imgPicture.Source);

            bitmap.SaveJpeg(myFileStream, bitmap.PixelWidth, bitmap.PixelHeight, 0, 100);
            myFileStream.Close();

            myFileStream = myStore.OpenFile(file, System.IO.FileMode.Open, System.IO.FileAccess.Read);

            var lib = new MediaLibrary();
            lib.SavePicture(file, myFileStream);

            toastDisplay = GlobalToastPrompt.CreateToastPrompt(
                "Success!",
                "Picture has been saved to your media library.");

            toastDisplay.Show();
        }

        private void MakeProfilePicture(object sender, EventArgs e)
        {
            User updatedData = new User();
            UserInfo u = App.MetrocamService.CurrentUser;

            updatedData.Biography = u.Biography;
            updatedData.CreatedDate = u.CreatedDate;
            updatedData.EmailAddress = u.EmailAddress;
            updatedData.FriendlyCreatedDate = u.FriendlyCreatedDate;
            updatedData.ID = u.ID;
            updatedData.Location = u.Location;
            updatedData.Name = u.Name;
            updatedData.Password = App.MetrocamService.HashPassword(Settings.password.Value);
            updatedData.Username = u.Username;

            // update ProfilePicture
            updatedData.ProfilePictureID = CurrentPicture.ID;

            App.MetrocamService.UpdateUserCompleted += new RequestCompletedEventHandler(MetrocamService_UpdateUserCompleted);
            GlobalLoading.Instance.IsLoading = true;
            App.MetrocamService.UpdateUser(updatedData);
        }

        void MetrocamService_UpdateUserCompleted(object sender, RequestCompletedEventArgs e)
        {
            App.MetrocamService.UpdateUserCompleted -= MetrocamService_UpdateUserCompleted;
            GlobalLoading.Instance.IsLoading = false;

            toastDisplay = GlobalToastPrompt.CreateToastPrompt(
                "Success!",
                "Your profile picture has been updated.");

            toastDisplay.Show();
        }

        void Favorite_Click(object sender, EventArgs e)
        {
            if (doingWork)
                return;

            FavoritedPicture data = new FavoritedPicture();
            data.PictureID = CurrentPicture.ID;
            data.UserID = App.MetrocamService.CurrentUser.ID;

            if ((from pic in App.FavoritedUserPictures where pic.ID.Equals(data.PictureID) select pic).SingleOrDefault() != null)
            {
                return;
            }

            App.FavoritedUserPictures.Add(CurrentPicture);

            GlobalLoading.Instance.IsLoading = true;

            App.MetrocamService.CreateFavoritedPictureCompleted += new RequestCompletedEventHandler(MetrocamService_CreateFavoritedPictureCompleted);
            App.MetrocamService.CreateFavoritedPicture(data);
        }

        void Unfavorite_Click(object sender, EventArgs e)
        {
            if (doingWork)
                return;

            GlobalLoading.Instance.IsLoading = true;

            App.FavoritedUserPictures.Remove(CurrentPicture);
            App.MetrocamService.DeleteFavoritedPictureCompleted += new RequestCompletedEventHandler(MetrocamService_DeleteFavoritedPictureCompleted);
            App.MetrocamService.DeleteFavoritedPicture(f);
        }

        void MetrocamService_DeleteFavoritedPictureCompleted(object sender, RequestCompletedEventArgs e)
        {
            doingWork = false;
            GlobalLoading.Instance.IsLoading = false;

            toastDisplay = GlobalToastPrompt.CreateToastPrompt(
                "Success!",
                "Picture has been removed from your favorites.");
            toastDisplay.Show();

            ApplicationBar.Buttons.RemoveAt(2);
            ApplicationBarIconButton favorite = new ApplicationBarIconButton(new Uri("Images/appbar.heart.png", UriKind.Relative));
            favorite.Text = "favorite";
            favorite.Click += new EventHandler(Favorite_Click);
            ApplicationBar.Buttons.Add(favorite);
                

            App.MetrocamService.DeleteFavoritedPictureCompleted -= MetrocamService_DeleteFavoritedPictureCompleted;
        }

        void MetrocamService_CreateFavoritedPictureCompleted(object sender, RequestCompletedEventArgs e)
        {
            f = e.Data as FavoritedPicture;

            doingWork = false;
            App.MetrocamService.CreateFavoritedPictureCompleted -= MetrocamService_CreateFavoritedPictureCompleted;
            GlobalLoading.Instance.IsLoading = false;

            ApplicationBar.Buttons.RemoveAt(2);
            ApplicationBarIconButton unfavorite = new ApplicationBarIconButton(new Uri("Images/appbar.heartbreak.png", UriKind.Relative));
            unfavorite.Text = "unfavorite";
            unfavorite.Click += new EventHandler(Unfavorite_Click);
            ApplicationBar.Buttons.Add(unfavorite);

            toastDisplay = GlobalToastPrompt.CreateToastPrompt(
                "Success!",
                "Picture has been added to your favorites.");

            toastDisplay.Show();
        }

        private void ViewUserDetail_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!NavigationContext.QueryString["type"].Equals("user"))
            {
                NavigationService.Navigate(new Uri("/UserDetailPage.xaml?id=" + CurrentPicture.ID + "&type=" + NavigationContext.QueryString["type"] + "&userid=" + CurrentPicture.User.ID, UriKind.Relative));
            }
        }

        private void Share(object sender, EventArgs e)
        {
            ShareLinkTask shareLinkTask = new ShareLinkTask();

            shareLinkTask.Title = "Shared via Metrocam";

            // replace with Web Application URL
            shareLinkTask.LinkUri = new Uri("http://metrocam.cloudapp.net/p/" + CurrentPicture.ID, UriKind.Absolute);
            shareLinkTask.Message = CurrentPicture.Caption;

            shareLinkTask.Show();
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HubTileService.UnfreezeGroup("PopularTiles");
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (GlobalLoading.Instance.IsLoading)
                GlobalLoading.Instance.IsLoading = false;
        }

        private void Flag_Click(object sender, EventArgs e)
        {
            toastDisplay = GlobalToastPrompt.CreateToastPrompt(
                "Success!",
                "Picture has been flagged for review.");

            toastDisplay.Show();
        }
    }
}