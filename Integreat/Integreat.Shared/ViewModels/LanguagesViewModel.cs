using Integreat.Shared.Models;
using Integreat.Shared.Utilities;
using Integreat.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Integreat.Shared.Data.Loader;
using Integreat.Shared.Services;
using Integreat.Shared.Services.Tracking;
using Xamarin.Forms;
using localization;

namespace Integreat.Shared
{
    public class LanguagesViewModel : BaseViewModel
    {

        private readonly Location _location;
        private readonly DataLoaderProvider _dataLoaderProvider;
        private IEnumerable<Language> _items;
        private string _errorMessage;
        private Language _selectedLanguage;

        private ICommand _loadLanguages;
        private ICommand _forceRefreshLanguagesCommand;
        private ICommand _onLanguageSelectedCommand;

        public LanguagesViewModel(IAnalyticsService analytics, Location location, DataLoaderProvider dataLoaderProvider, INavigator navigator)
            : base(analytics)
        {
            Title = AppResources.Language;
            navigator.HideToolbar(this);

            Items = new ObservableCollection<Language>();
            _location = location;
            _dataLoaderProvider = dataLoaderProvider;
        }

        public string Description { get; set; }

        public Location Location => _location;

        public Language SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                SetProperty(ref _selectedLanguage, value);
                if (value != null)
                    LanguageSelected();
            }
        }

        public ICommand OnLanguageSelectedCommand
        {
            get => _onLanguageSelectedCommand;
            set => SetProperty(ref _onLanguageSelectedCommand, value);
        }
        public ICommand LoadLanguagesCommand => _loadLanguages ?? (_loadLanguages = new Command(() => ExecuteLoadLanguages()));
        public ICommand ForceRefreshLanguagesCommand => _forceRefreshLanguagesCommand ?? (_forceRefreshLanguagesCommand = new Command(() => ExecuteLoadLanguages(true)));


        /// <summary>
        /// Gets or sets the error message that a view may display.
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                SetProperty(ref _errorMessage, value);
                OnPropertyChanged(nameof(ErrorMessageVisible));
            }
        }

        /// <summary>
        /// Gets a value indicating whether the [error message should be visible].
        /// </summary>
        public bool ErrorMessageVisible => !string.IsNullOrWhiteSpace(ErrorMessage);

        public IEnumerable<Language> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private void LanguageSelected()
        {
            Preferences.SetLanguage(_location, SelectedLanguage);
            OnLanguageSelectedCommand?.Execute(this);
        }
        public override void OnAppearing()
        {
            ExecuteLoadLanguages();
            base.OnAppearing();
        }

        protected override void OnMetadataChanged()
        {
            ExecuteLoadLanguages(true);
        }

        public override void OnRefresh(bool force = false)
        {
            ExecuteLoadLanguages(force);
        }

        private async void ExecuteLoadLanguages(bool forceRefresh = false)
        {
            if (IsBusy){ return; }
            try
            {
                IsBusy = true;
                // get the languages as list, then sort them
                var asList = new List<Language>(await _dataLoaderProvider.LanguagesDataLoader.Load(forceRefresh, _location, err => ErrorMessage = err));
                asList.Sort(CompareLanguage);
                // set the loaded Languages
                Items = asList;
            }
            finally
            {
                IsBusy = false;
            }
            Debug.WriteLine(AppResources.Languages_loaded);
        }

        private static int CompareLanguage(Language a, Language b)
        {
            return string.Compare(a.Name, b.Name, StringComparison.Ordinal);
        }
    }
}

