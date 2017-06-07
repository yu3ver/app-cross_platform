﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Integreat.Shared.Data.Loader;
using Integreat.Shared.Models;
using Integreat.Shared.Services;
using Integreat.Shared.Services.Tracking;
using Integreat.Shared.Utilities;
using Integreat.Shared.ViewModels.Resdesign;
using Integreat.Shared.ViewModels.Resdesign.General;
using localization;
using Xamarin.Forms;

namespace Integreat.Shared {
    public class Careers4RefugeesViewModel : BaseContentViewModel {
        #region Fields
        private INavigator _navigator;

        private ObservableCollection<Careers4RefugeesTemp.CareerOffer> _offers;
        private bool _hasNoResults;
        private Func<string, bool, GeneralWebViewPageViewModel> _generalWebViewFactory; // factory generated by AutoFac to resolve a GeneralWebViewPageViewModel instance

        #endregion

        #region Properties
        public ObservableCollection<Careers4RefugeesTemp.CareerOffer> Offers {
            get {
                return _offers;
            }
            private set {
                SetProperty(ref _offers, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has no results for the given location or not.
        /// </summary>
        public bool HasNoResults {
            get { return _hasNoResults; }
            set { SetProperty(ref _hasNoResults, value); }
        }

        /// <summary>
        /// Gets the label if this instance has no results for the given location.
        /// </summary>
        public string HasNoResultsLabel => AppResources.HasNoResults;

        /// <summary>
        /// The displayed header image on the page
        /// </summary>
        public string HeaderImage { get; set; }
        #endregion⁄


        public Careers4RefugeesViewModel(IAnalyticsService analytics, INavigator navigator, DataLoaderProvider dataLoaderProvider, Func<string, bool, GeneralWebViewPageViewModel> generalWebViewFactory )
            : base(analytics, dataLoaderProvider) {
            Title = "Extras";
            HeaderImage = "c4r_logo";
            _navigator = navigator;
            _generalWebViewFactory = generalWebViewFactory;
            _navigator.HideToolbar(this);
        }

        protected override async void LoadContent(bool forced = false, Language forLanguage = null, Location forLocation = null) {
            // wait until this resource is free
            Offers?.Clear();
            await Task.Run(() => {
                while (IsBusy)
                {
                    //empty
                }
            });
            IsBusy = true;
            HasNoResults = false;
            if (forLocation == null) forLocation = LastLoadedLocation;
            if (forLanguage == null) forLanguage = LastLoadedLanguage;

            var url = forLocation.Careers4RefugeesUrl;
            try {
                var offers =
                    new ObservableCollection<Careers4RefugeesTemp.CareerOffer>(
                        await XmlWebParser.ParseXmlFromAddressAsync<List<Careers4RefugeesTemp.CareerOffer>>(url, "anzeigen"));

                // hook up the command
                foreach (var careerOffer in offers)
                {
                    careerOffer.OnTapCommand = new Command(OnOfferTapped);
                }

                Offers = offers;
            } catch (Exception e) {
                Debug.WriteLine("C4R Error: " + e);
                HasNoResults = true;
            } finally { IsBusy = false; }
        }

        /// <summary>
        /// Called when an [offer is tapped]. (By a command)
        /// </summary>
        /// <param name="careerOfferObject">The career offer object.</param>
        private async void OnOfferTapped(object careerOfferObject)
        {
            // try to cast the object, abort if failed
            var careerOffer = careerOfferObject as Careers4RefugeesTemp.CareerOffer;
            if (careerOffer == null) return;
            var view = _generalWebViewFactory(careerOffer.Link, false);
            view.Title = "Career4Refugees";
            careerOffer.IsVisitedImage = "Icon_Small";
            // push a new general webView page, which will show the URL of the offer
            await _navigator.PushAsync(view, Navigation);
        }
    }
}
