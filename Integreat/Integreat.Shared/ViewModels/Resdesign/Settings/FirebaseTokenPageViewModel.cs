using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Integreat.Shared.Data.Loader;
using Integreat.Shared.Models;
using Integreat.Shared.Services;
using Integreat.Shared.Services.Tracking;
using Integreat.Shared.Utilities;
using localization;
using Xamarin.Forms;

namespace Integreat.Shared.ViewModels.Resdesign.Settings
{
    public class FirebaseTokenPageViewModel : BaseContentViewModel
    {
        #region Fields
        private IEnumerable<Location> _locations;

        #endregion


        #region Properties

        private List<Location> _availableLocations;
        /// <summary>
        /// Gets or sets the available locations to receive push notifications for.
        /// </summary>
        public List<Location> AvailableLocations {
            get => _availableLocations;
            set {
                SetProperty(ref _availableLocations, value);
                // raise property changed event for groupedLocation (as it relies on FoundLocations)
                OnPropertyChanged(nameof(GroupedLocations));
            }
        }

        /// <summary>
        /// The FoundLocations, but grouped after the GroupKey property (which is the first letter of the name).
        /// </summary>
        public List<Grouping<string, Location>> GroupedLocations => AvailableLocations == null ? null : (from location in AvailableLocations
            group location by location.GroupKey into locationGroup
            select new Grouping<string, Location>(locationGroup.Key, locationGroup)).ToList();


        private Location _selectedLocation;
        /// <summary>
        /// Gets or sets the selected location. Will be set by the list every time the user clicks on an entry.
        /// </summary>
        public Location SelectedLocation {
            get => _selectedLocation;
            set {
                if (!SetProperty(ref _selectedLocation, value)) return;
                if (_selectedLocation != null)
                {
                    LocationSelected();
                }
            }
        }

        /// <summary>
        /// Gets the push description.
        /// </summary>
        public string PushText => AppResources.FirebaseDescription;

        /// <summary>
        /// Gets the search placeholder text.
        /// </summary>
        public string SearchPlaceholderText => AppResources.Search;

        #endregion

        public FirebaseTokenPageViewModel(IAnalyticsService analyticsService, DataLoaderProvider dataLoaderProvider) : base(analyticsService, dataLoaderProvider)
        {
            Title = AppResources.FirebaseName;
        }

        /// <summary>
        /// Called when the user pressed on a location.
        /// </summary>
        private async void LocationSelected()
        {

        }

        public override void OnAppearing()
        {
            LoadContent();
            base.OnAppearing();
        }


        protected override async void LoadContent(bool forced = false, Language forLanguage = null, Location forLocation = null)
        {
            if (IsBusy) { return; }
            try
            {
                IsBusy = true;
                // clear list (call property changed, as the FoundLocations property indirectly affects the GroupedLocations, which are the locations displayed)
                AvailableLocations?.Clear();
                OnPropertyChanged(nameof(GroupedLocations));
                // put locations into list and sort them.
                var asList = new List<Location>(await _dataLoaderProvider.LocationsDataLoader.Load(forced, err => ErrorMessage = err));
                asList.Sort(CompareLocations);
                // then set the field
                _locations = asList;
                Search();
            }
            finally
            {
                IsBusy = false;
            }

        }


        public static int CompareLocations(Location a, Location b)
        {
            return string.Compare(a.NameWithoutStreetPrefix, b.NameWithoutStreetPrefix, StringComparison.Ordinal);
        }

        #region View Data

        private string _searchText = string.Empty;
        public string SearchText {
            get => _searchText;
            set {
                if (SetProperty(ref _searchText, value))
                {
                    Search();
                }
            }
        }

        #endregion
        

        public void Search()
        {
            AvailableLocations = _locations?.Where(x => x.Find(SearchText)).ToList();
        }
    }
}
