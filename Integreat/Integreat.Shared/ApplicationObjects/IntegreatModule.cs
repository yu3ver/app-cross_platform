﻿using Autofac;
using Integreat.Shared.ViewModels;
using System;
using System.Net.Http;
using System.Security;
using Integreat.Shared.Data;
using Integreat.Shared.Data.Loader;
using Integreat.Shared.Data.Loader.Targets;
using Integreat.Shared.Pages;
using Integreat.Shared.Pages.Redesign;
using Integreat.Shared.Pages.Redesign.Events;
using Integreat.Shared.Pages.Redesign.General;
using Integreat.Shared.Pages.Redesign.Main;
using Integreat.Shared.Pages.Redesign.Settings;
using Integreat.Shared.ViewModels.Resdesign;
using Integreat.Shared.ViewModels.Resdesign.Events;
using Integreat.Shared.ViewModels.Resdesign.General;
using Integreat.Shared.ViewModels.Resdesign.Settings;
using Integreat.Utilities;
using ModernHttpClient;
using Newtonsoft.Json;
using Refit;
using Xamarin.Forms;
using Debug = System.Diagnostics.Debug;
using Page = Xamarin.Forms.Page;

namespace Integreat.Shared.ApplicationObjects
{
    [SecurityCritical]
    public class IntegreatModule : Module
    {
        [SecurityCritical]
        public IntegreatModule()
        {
        }

        [SecurityCritical]
        protected override void Load(ContainerBuilder builder)
        {

            //
            // VIEW MODELS
            // 

            builder.RegisterType<PageViewModel>();
            builder.RegisterType<EventPageViewModel>();


            builder.RegisterType<LocationsViewModel>();
            builder.RegisterType<LanguagesViewModel>(); // can have multiple instances


            builder.RegisterType<SearchViewModel>();
            // redesign
            builder.RegisterType<ContentContainerViewModel>();
            builder.RegisterType<MainContentPageViewModel>();
            builder.RegisterType<ExtrasContentPageViewModel>();
            builder.RegisterType<EventsContentPageViewModel>();

            // main
            builder.RegisterType<MainTwoLevelViewModel>();

            // general
            builder.RegisterType<GeneralWebViewPageViewModel>();
            builder.RegisterType<PdfWebViewPageViewModel>();
            builder.RegisterType<ImagePageViewModel>();

            // settings
            builder.RegisterType<SettingsPageViewModel>();

            //
            // PAGES
            //

            // register views
            builder.RegisterType<LanguagesPage>();
            builder.RegisterType<LocationsPage>();
            builder.RegisterType<SearchListPage>();
            // redesign
            builder.RegisterType<ContentContainerPage>();
            builder.RegisterType<MainContentPage>();
            builder.RegisterType<ExtrasContentPage>();
            builder.RegisterType<EventsContentPage>();

            // main
            builder.RegisterType<MainTwoLevelPage>();

            // events
            builder.RegisterType<EventsSingleItemDetailPage>();
            builder.RegisterType<EventsSingleItemDetailViewModel>();

            // extras
            builder.RegisterType<JobOffersPage>();
            builder.RegisterType<Careers4RefugeesViewModel>();
            builder.RegisterType<SprungbrettViewModel>();

            // general
            builder.RegisterType<GeneralWebViewPage>();
            builder.RegisterType<PdfWebViewPage>();
            builder.RegisterType<ImageViewPage>();

            // settings
            builder.RegisterType<SettingsPage>();

            // current page resolver
            builder.RegisterInstance<Func<Page>>(Instance);

            builder.RegisterInstance(CreateDataLoadService());
            builder.RegisterType<DataLoaderProvider>();
            builder.RegisterType<LocationsDataLoader>();
            builder.RegisterType<LanguagesDataLoader>();
            builder.RegisterType<DisclaimerDataLoader>();
            builder.RegisterType<EventPagesDataLoader>();
            builder.RegisterType<PagesDataLoader>();
        }

        [SecurityCritical]
        private static IDataLoadService CreateDataLoadService()
        {
            var networkServiceSettings = new RefitSettings
            {
                JsonSerializerSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Error = (sender, args) => Debug.WriteLine(args)
                    //, TraceWriter = new ConsoleTraceWriter() // debug tracer to see the json input
                }
            };

            var client = new HttpClient(new NativeMessageHandler())
            {
                BaseAddress = new Uri(Constants.IntegreatReleaseUrl)
            };

            return RestService.For<IDataLoadService>(client, networkServiceSettings);
        }
        [SecurityCritical]
        private static Page Instance() => Application.Current.MainPage;
    }
}
