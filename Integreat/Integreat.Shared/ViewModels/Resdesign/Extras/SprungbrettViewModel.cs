﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Integreat.Shared.Data.Loader;
using Integreat.Shared.Models;
using Integreat.Shared.Models.Sprungbrett;
using Integreat.Shared.Services;
using Integreat.Shared.Services.Tracking;
using Integreat.Shared.Utilities;
using Integreat.Shared.ViewModels.Resdesign;
using Integreat.Shared.ViewModels.Resdesign.General;
using Xamarin.Forms;

namespace Integreat.Shared
{
    public class SprungbrettViewModel : BaseContentViewModel
    {
        #region Fields
        private readonly INavigator _navigator;

        private ObservableCollection<SprungbrettJobOffer> _offers;
        private bool _hasNoResults;
        private readonly Func<string, bool, GeneralWebViewPageViewModel> _generalWebViewFactory; // factory generated by AutoFac to resolve a GeneralWebViewPageViewModel instance
        #endregion

        #region Properties
        public ObservableCollection<SprungbrettJobOffer> Offers
        {
            get
            {
                return _offers;
            }
            private set
            {
                SetProperty(ref _offers, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has no results for the given location or not.
        /// </summary>
        public bool HasNoResults
        {
            get { return _hasNoResults; }
            set { SetProperty(ref _hasNoResults, value); }
        }

        /// <summary>
        /// The displayed header image on the page
        /// </summary>
        public string HeaderImage { get; set; }
        #endregion⁄

        public SprungbrettViewModel(IAnalyticsService analytics, INavigator navigator, DataLoaderProvider dataLoaderProvider, Func<string, bool, GeneralWebViewPageViewModel> generalWebViewFactory)
            : base(analytics, dataLoaderProvider)
        {
            Title = "Sprungbrett";
            HeaderImage = "sbi-logo.png";
            _navigator = navigator;
            _generalWebViewFactory = generalWebViewFactory;

            _navigator.HideToolbar(this);
        }

        protected override async void LoadContent(bool forced = false, Language forLanguage = null, Location forLocation = null)
        {
            Offers?.Clear();
            // wait until this resource is free
            await Task.Run(() =>
            {
                while (IsBusy) { /*empty body*/ }
            });
            IsBusy = true;
            HasNoResults = true;

            if (forLocation == null) forLocation = LastLoadedLocation;

            var url = forLocation.SprungbrettUrl;

            if (string.IsNullOrEmpty(url))
            {
                url = "https://www.sprungbrett-intowork.de/ajax/app-search-internships";
            }
            try
            {
                var json = await new SprungbrettTemp().FetchJobOffersAsync(url);

                var offers = new ObservableCollection<SprungbrettJobOffer>(json.JobOffers);

                foreach (var jobOffer in offers)
                {
                    jobOffer.OnTapCommand = new Command(OnOfferTapped);
                }

                Offers = offers;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                HasNoResults = true;
            }
            finally { IsBusy = false; }

            IsBusy = false;
        }

        /// <summary>
        /// Called when an [offer is tapped]. (By a command)
        /// </summary>
        /// <param name="jobOfferObject">The job offer object.</param>
        private async void OnOfferTapped(object jobOfferObject)
        {
            // try to cast the object, abort if failed
            var jobOffer = jobOfferObject as SprungbrettJobOffer;
            if (jobOffer == null) return;

            var view = _generalWebViewFactory(jobOffer.Url, false);
            view.Title = "Sprungbrett";
            // push a new general webView page, which will show the URL of the offer
            await _navigator.PushAsync(view, Navigation);
        }
    }
}
