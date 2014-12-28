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
        // TripPin instances
        private static Person me = null;
        private static string myPhotoUri = null;
        private static int numMyTrips = -1;
        private static int numMyFriends = -1;
        private static int numPeople = -1;
        private static int numPhotos = -1;
        private static int numAirports = -1;
        private static int numAirlines = -1;

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
            if (me == null)
            {
                me = await App.tripPinContext.Me.GetValueAsync();
            }
            if (myPhotoUri == null)
            {
                myPhotoUri = App.serviceUri + "$value";
            }
            if (numMyTrips < 0)
            {
                numMyTrips = 2;
            }
            if (numMyFriends < 0)
            {
                numMyFriends = 3;
            }
            if (numPeople < 0)
            {
                numPeople = 4;
            }
            if (numPhotos < 0)
            {
                numPhotos = 5;
            }
            if (numAirlines < 0)
            {
                numAirlines = 6;
            }
            if (numAirports < 0)
            {
                numAirports = 7;
            }

            this.DefaultViewModel["Me"] = me;
            this.DefaultViewModel["MyPhotoUri"] = myPhotoUri;
            this.DefaultViewModel["NumMyTrips"] = numMyTrips.ToString() + " trips";
            this.DefaultViewModel["NumMyFriends"] = numMyFriends.ToString() + " friends";
            this.DefaultViewModel["NumPeople"] = numPeople.ToString() + " people";
            this.DefaultViewModel["NumPhotos"] = numPhotos.ToString() + " photos";
            this.DefaultViewModel["NumAirlines"] = numAirlines.ToString() + " airlines";
            this.DefaultViewModel["NumAirports"] = numAirports.ToString() + " airports";
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
