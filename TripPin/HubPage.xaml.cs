using Windows.Storage;
using Windows.UI.Popups;
using TripPin.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using TripPin.DataModel;
using Microsoft.OData.Client;

// The Hub Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace TripPin
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        public HubPage()
        {
            this.InitializeComponent();

            // Hub is only supported in Portrait orientation
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // Exception
            Exception exception = null;

            // Me
            string myFullName = null;
            string myUserName = null;
            string myGender = null;
            string myEmail = null;

            if (!App.localSettings.Values.ContainsKey("MyEmail"))
            {
                Person me = null;
                try
                {
                    me = await App.tripPinContext.Me.GetValueAsync();
                }
                catch (Exception localException)
                {
                    exception = localException;
                }

                if (exception == null)
                {
                    myFullName = me.FirstName + " " + me.LastName;
                    myGender = me.Gender.ToString();
                    myUserName = me.UserName;
                    myEmail = me.Emails[0];
                    App.localSettings.Values["MyFullName"] = myFullName;
                    App.localSettings.Values["MyGender"] = myGender;
                    App.localSettings.Values["MyUserName"] = myUserName;
                    App.localSettings.Values["MyEmail"] = myEmail;
                }
                else
                {
                    await new MessageDialog(exception.Message, "Error loading Me").ShowAsync();
                }
            }
            else
            {
                myFullName = App.localSettings.Values["MyFullName"].ToString();
                myGender = App.localSettings.Values["MyGender"].ToString();
                myUserName = App.localSettings.Values["MyUserName"].ToString();
                myEmail = App.localSettings.Values["MyEmail"].ToString();
            }

            this.DefaultViewModel["MyFullName"] = myFullName;
            this.DefaultViewModel["MyUserName"] = myUserName;
            this.DefaultViewModel["MyGender"] = myGender;
            this.DefaultViewModel["MyEmail"] = myEmail;

            // Counts
            await Utilities.Utilities.AssignCountToPageDataAsync("Me/Friends/$count", "NumMyFriends", DefaultViewModel, " friends");
            await Utilities.Utilities.AssignCountToPageDataAsync("Me/Trips/$count", "NumMyTrips", DefaultViewModel, " trips");
            await Utilities.Utilities.AssignCountToPageDataAsync("People/$count", "NumPeople", DefaultViewModel, " people");
            await Utilities.Utilities.AssignCountToPageDataAsync("Photos/$count", "NumPhotos", DefaultViewModel, " photos");
            await Utilities.Utilities.AssignCountToPageDataAsync("Airlines/$count", "NumAirlines", DefaultViewModel, " airlines");
            await Utilities.Utilities.AssignCountToPageDataAsync("Airports/$count", "NumAirports", DefaultViewModel, " airports");

            // MyPhotoUri
            StorageFile photoFile = null;
            try
            {
                photoFile = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("MyPhoto.jpg");
            }
            catch (FileNotFoundException fileNotFoundException)
            {

            }
            if (photoFile == null)
            {
                var photoStream =
                    (await
                        App.tripPinContext.GetReadStreamAsync(await App.tripPinContext.Me.Photo.GetValueAsync(),
                            new DataServiceRequestArgs() { AcceptContentType = "*/*" })).Stream;
                photoFile = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("MyPhoto.jpg");
                using (var outputStream = await photoFile.OpenStreamForWriteAsync())
                {
                    await photoStream.CopyToAsync(outputStream);
                }
                this.DefaultViewModel["MyPhotoUri"] = photoFile.Path;
            }
            else
            {
                this.DefaultViewModel["MyPhotoUri"] = photoFile.Path;
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: Save the unique state of the page here.
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void AirlinesTile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!Frame.Navigate(typeof(AirlinesPage)))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }
    }
}
