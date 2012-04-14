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
using MobileClientLibrary.Models;
using MobileClientLibrary;
using System.Windows.Navigation;
using JeffWilcox.FourthAndMayor;
using Coding4Fun.Phone.Controls;

namespace MetrocamPan
{
    public partial class SignUpPage : PhoneApplicationPage
    {
        private User currentUser;

        private ToastPrompt toastDisplay;

        public SignUpPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            App.isFromLandingPage = true;

            this.LocationInput.Hint = "optional";
            this.BiographyInput.Hint = "optional";
        }

        private void UsernameInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Switch focus to the whole frame
                Dispatcher.BeginInvoke(() =>
                    this.Focus());
            }
        }

        private void FullnameInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Switch focus to the whole frame
                Dispatcher.BeginInvoke(() =>
                    this.Focus());
            }
        }

        private void PasswordInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Switch focus to the whole frame
                Dispatcher.BeginInvoke(() =>
                    this.Focus());
            }
        }

        private void ConfirmPasswordInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Switch focus to the whole frame
                Dispatcher.BeginInvoke(() =>
                    this.Focus());
            }
        }

        private void EmailInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Switch focus to the whole frame
                Dispatcher.BeginInvoke(() =>
                    this.Focus());
            }
        }

        private void LocationInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Switch focus to the whole frame
                Dispatcher.BeginInvoke(() =>
                    this.Focus());
            }
        }

        private void BiographyInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Switch focus to the whole frame
                Dispatcher.BeginInvoke(() =>
                    this.Focus());
            }
        }

        private static ToastPrompt GetBasicToast(string title = "Basic")
        {
            return new ToastPrompt
            {
                Title = title,
                Message = "Please enter text here"
            };
        }

        private void Accept_Click(object sender, EventArgs e)
        {
            // Validate Input
            //      First checks username valid?
            //      Then checks name empty?
            //      Then checks password similar?
            //      Then checks password strong?
            //      Then checks email valid?
            if (!InputValidator.isValidUsername(this.UsernameInput.Text) ||
                !InputValidator.isNonEmpty(this.FullnameInput.Text, "full name") ||
                !InputValidator.isPasswordSame(this.PasswordInput.Password, this.ConfirmPasswordInput.Password) ||
                !InputValidator.isStrongPassword(this.PasswordInput.Password) ||
                !InputValidator.isValidEmail(this.EmailInput.Text))
            {
                // Do nothing
                return;
            }

            currentUser = new User();
            currentUser.Username = this.UsernameInput.Text;
            currentUser.Name = this.FullnameInput.Text;
            currentUser.Password = this.PasswordInput.Password;
            currentUser.EmailAddress = this.EmailInput.Text;

            // Set default values for optional fields if empty
            if (this.LocationInput.Text.Length == 0)
            {
                currentUser.Location = "Metrocam City";
            }
            else
            {
                currentUser.Location = this.LocationInput.Text;
            }

            if (this.BiographyInput.Text.Length == 0)
            {
                currentUser.Biography = "Metrocam for life!";
            }
            else
            {
                currentUser.Biography = this.BiographyInput.Text;
            }

            // Subscribe event to CreateUserCompleted
            App.MetrocamService.CreateUserCompleted += new MobileClientLibrary.RequestCompletedEventHandler(MetrocamService_CreateUserCompleted);

            GlobalLoading.Instance.IsLoading = true;

            // Calls CreateUser to WebService
            App.MetrocamService.CreateUser(currentUser);
        }

        private void MetrocamService_CreateUserCompleted(object sender, RequestCompletedEventArgs e)
        {
            // Unsubscribe
            App.MetrocamService.CreateUserCompleted -= new MobileClientLibrary.RequestCompletedEventHandler(MetrocamService_CreateUserCompleted);

            // Now we authenticate using the username and password
            App.MetrocamService.AuthenticateCompleted += new RequestCompletedEventHandler(MetrocamService_AuthenticateCompleted);

            // Calls Authenticate to obtain UserInfo object
            App.MetrocamService.Authenticate(this.UsernameInput.Text, this.PasswordInput.Password);
        }

        void MetrocamService_AuthenticateCompleted(object sender, RequestCompletedEventArgs e)
        {
            // Unsubscribe
            App.MetrocamService.AuthenticateCompleted -= new RequestCompletedEventHandler(MetrocamService_AuthenticateCompleted);

            GlobalLoading.Instance.IsLoading = false;

            UserInfo obtainedUser = App.MetrocamService.CurrentUser;

            // Load user specific settings
            Settings.getUserSpecificSettings(obtainedUser.Username);

            // Store into isolated storage
            Settings.isLoggedIn.Value = true;
            Settings.username.Value = currentUser.Username;
            Settings.password.Value = this.PasswordInput.Password;      // As of now, currentUser.Password returns a hashed password.

            App.isFromLandingPage = true;
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (GlobalLoading.Instance.IsLoading)
                GlobalLoading.Instance.IsLoading = false;
        }

        private void UsernameInput_GotFocus(object sender, RoutedEventArgs e)
        {
            // Set properties of ToastPrompt
            toastDisplay = GetBasicToast("Valid usernames:");
            toastDisplay.Message = "Between 4-12 characters.\nNo special characters.\nNo spaces.";
            toastDisplay.MillisecondsUntilHidden = 10000;
            toastDisplay.TextWrapping = TextWrapping.Wrap;
            
            toastDisplay.Show();
        }

        private void UsernameInput_LostFocus(object sender, RoutedEventArgs e)
        {
            toastDisplay.Hide();
        }

        private void PasswordInput_GotFocus(object sender, RoutedEventArgs e)
        {
            // Set properties of ToastPrompt
            toastDisplay = GetBasicToast("Valid passwords:");
            toastDisplay.Message = "Between 6-12 characters.\nNo special characters.\nNo spaces\nAt least one capital letter.\nAt least one number.";
            toastDisplay.MillisecondsUntilHidden = 10000;
            toastDisplay.TextWrapping = TextWrapping.Wrap;
            
            toastDisplay.Show();
        }

        private void PasswordInput_LostFocus(object sender, RoutedEventArgs e)
        {
            toastDisplay.Hide();
        }

        private void FullnameInput_GotFocus(object sender, RoutedEventArgs e)
        {
            // Set properties of ToastPrompt
            toastDisplay = GetBasicToast("Full Name:");
            toastDisplay.Message = "You will not be able to change this later.";
            toastDisplay.MillisecondsUntilHidden = 10000;
            toastDisplay.TextWrapping = TextWrapping.Wrap;

            toastDisplay.Show();
        }

        private void FullnameInput_LostFocus(object sender, RoutedEventArgs e)
        {
            toastDisplay.Hide();
        }

        private void EmailInput_GotFocus(object sender, RoutedEventArgs e)
        {
            // Set properties of ToastPrompt
            toastDisplay = GetBasicToast("Email:");
            toastDisplay.Message = "We will use this email address to verify you.";
            toastDisplay.MillisecondsUntilHidden = 10000;
            toastDisplay.TextWrapping = TextWrapping.Wrap;

            toastDisplay.Show();
        }

        private void EmailInput_LostFocus(object sender, RoutedEventArgs e)
        {
            toastDisplay.Hide();
        }
    }
}