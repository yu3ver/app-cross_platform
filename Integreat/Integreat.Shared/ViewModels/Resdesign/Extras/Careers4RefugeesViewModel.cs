﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Integreat.Shared.Data.Loader;
using Integreat.Shared.Models;
using Integreat.Shared.Services;
using Integreat.Shared.Services.Persistence;
using Integreat.Shared.Services.Tracking;
using Integreat.Shared.Utilities;
using Integreat.Shared.ViewModels;
using Integreat.Shared.ViewModels.Resdesign;
using Integreat.Shared.ViewModels.Resdesign.General;
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

        #endregion⁄


        public Careers4RefugeesViewModel(IAnalyticsService analytics, INavigator navigator, DataLoaderProvider dataLoaderProvider, Func<string, bool, GeneralWebViewPageViewModel> generalWebViewFactory )
            : base(analytics, dataLoaderProvider) {
            Title = "Extras";
            _navigator = navigator;
            _generalWebViewFactory = generalWebViewFactory;
            _navigator.HideToolbar(this);
        }

        public override async void LoadContent(bool forced = false, Language forLanguage = null, Location forLocation = null) {
            // wait until this resource is free
            Offers?.Clear();
            await Task.Run(() => {
                while (IsBusy) ;
            });
            IsBusy = true;
            HasNoResults = false;
            if (forLocation == null) forLocation = LastLoadedLocation;
            if (forLanguage == null) forLanguage = LastLoadedLanguage;

            string url;
            switch (forLocation.Name.ToLower()) {
                case "stadt regensburg":
                    url = "http://www.careers4refugees.de/jobsearch/exports/integreat_regensburg";
                    break;
                case "bad tölz":
                    url = "http://www.careers4refugees.de/jobsearch/exports/integreat_bad-toelz";
                    break;
                case "landkreis germersheim":
                    url = "http://www.careers4refugees.de/jobsearch/exports/integreat_gemersheim";
                    break;
                case "köln":
                    url = "http://www.careers4refugees.de/jobsearch/exports/koeln";
                    break;
                case "bochum":
                    url = "http://www.careers4refugees.de/jobsearch/exports/bochum";
                    break;

                /*
                    url = "http://www.careers4refugees.de/jobsearch/exports/integreat_"+_lastLoadedLocation.Name.ToLower();
                    Dormagen http://www.careers4refugees.de/jobsearch/exports/integreat_dormagen
                    Ahaus http://www.careers4refugees.de/jobsearch/exports/integreat_ahaus
                    Main-Taunus-Kreis http://www.careers4refugees.de/jobsearch/exports/integreat_main-taunus-kreis
                    Regensburg http://www.careers4refugees.de/jobsearch/exports/integreat_regensburg
                    Kissing http://www.careers4refugees.de/jobsearch/exports/integreat_kissing
                    Bad Tölz http://www.careers4refugees.de/jobsearch/exports/integreat_bad-toelz
                    Augsburg http://www.careers4refugees.de/jobsearch/exports/integreat_augsburg
                    http://www.careers4refugees.de/jobsearch/exports/integreat_gemersheim 
                    http://www.careers4refugees.de/jobsearch/exports/bochum 
                    http://www.careers4refugees.de/jobsearch/exports/koeln 
                */
                default:
                    url = "http://www.careers4refugees.de/jobsearch/exports/integreat_" + forLocation.Name.ToLower();
                    break;
            }

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

            // push a new general webView page, which will show the URL of the offer
            await _navigator.PushAsync(_generalWebViewFactory(careerOffer.Link, false), Navigation);
        }
    }
}
