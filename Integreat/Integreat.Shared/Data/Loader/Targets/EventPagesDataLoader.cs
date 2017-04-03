﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integreat.Shared.Models;
using Integreat.Shared.Utilities;

namespace Integreat.Shared.Data.Loader.Targets {
    public class EventPagesDataLoader : IDataLoader {
        public const string FileNameConst = "events";
        public string FileName => FileNameConst;
        public DateTime LastUpdated {
            get { return Preferences.LastPageUpdateTime<EventPage>(_lastLoadedLanguage, _lastLoadedLocation); }
            set { Preferences.SetLastPageUpdateTime<EventPage>(_lastLoadedLanguage, _lastLoadedLocation, DateTime.Now); }
        }

        public string Id => "Id";

        private readonly IDataLoadService _dataLoadService;
        private Location _lastLoadedLocation;
        private Language _lastLoadedLanguage;

        public EventPagesDataLoader(IDataLoadService dataLoadService) {
            _dataLoadService = dataLoadService;
        }


        public Task<Collection<EventPage>> Load(bool forceRefresh, Language forLanguage, Location forLocation) {
            _lastLoadedLocation = forLocation;
            _lastLoadedLanguage = forLanguage;

            Action<Collection<EventPage>> worker = pages => {
                foreach (var page in pages) {
                    page.PrimaryKey = Page.GenerateKey(page.Id, forLocation, forLanguage);
                    page.LanguageId = forLanguage.PrimaryKey;
                    page.Language = forLanguage;
                    if (!"".Equals(page.ParentJsonId) && page.ParentJsonId != null) {
                        page.ParentId = Page.GenerateKey(page.ParentJsonId, forLocation, forLanguage);
                    }
                    page.AvailableLanguages?.ForEach(x => {
                        var language =
                            forLocation.Languages.FirstOrDefault(y => string.Equals(y.ShortName, x.LanguageId));
                        x.LanguageId = language?.PrimaryKey;
                        x.OtherPageId = Page.GenerateKey(x.OtherPageId, forLocation,
                            language);
                        x.OwnPageId = page.PrimaryKey;
                    });
                }
            };

            return DataLoaderProvider.ExecuteLoadMethod(forceRefresh, this, () => _dataLoadService.GetEventPages(forLanguage, forLocation, new UpdateTime(LastUpdated.Ticks)), worker);
        }
    }
}
