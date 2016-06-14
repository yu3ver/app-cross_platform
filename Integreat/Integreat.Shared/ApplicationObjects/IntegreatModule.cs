﻿using Autofac;
using Integreat.Shared.Services.Loader;
using Integreat.Shared.ViewModels;
using System;
using System.Net.Http;
using Fusillade;
using Integreat.Shared.Pages;
using Integreat.Shared.Services.Network;
using ModernHttpClient;
using Newtonsoft.Json;
using Refit;
using Xamarin.Forms;
using NavigationPage = Xamarin.Forms.NavigationPage;
using Page = Xamarin.Forms.Page;

namespace Integreat.Shared.ApplicationObjects
{
    public class IntegreatModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance<Func<Priority, INetworkService>>(Instance);

            // register loader
            builder.RegisterType<PageLoader>();
            builder.RegisterType<LocationsLoader>().SingleInstance();
            builder.RegisterType<LanguagesLoader>();
            builder.RegisterType<EventPageLoader>();
            builder.RegisterType<DisclaimerLoader>();

            // register view models
            builder.RegisterType<PagesViewModel>().SingleInstance();
            builder.RegisterType<DetailedPagesViewModel>();
            builder.RegisterType<PageViewModel>();

            builder.RegisterType<EventPagesViewModel>().SingleInstance();
            builder.RegisterType<EventPageViewModel>();

            builder.RegisterType<DisclaimerViewModel>();

            builder.RegisterType<LocationsViewModel>().SingleInstance();
            builder.RegisterType<LanguagesViewModel>(); // can have multiple instances

            builder.RegisterType<NavigationViewModel>().SingleInstance();
            builder.RegisterType<TabViewModel>().SingleInstance();
            builder.RegisterType<MainPageViewModel>().SingleInstance();

            builder.RegisterType<SearchViewModel>();

            // register views
            builder.RegisterType<EventDetailPage>();
            builder.RegisterType<EventsOverviewPage>();
            builder.RegisterType<InformationOverviewPage>();
            builder.RegisterType<DetailedInformationPage>();
            builder.RegisterType<DisclaimerListPage>();
            builder.RegisterType<LanguagesPage>();
            builder.RegisterType<LocationsPage>();
            builder.RegisterType<MainPage>();
            builder.RegisterType<NavigationDrawerPage>();
            builder.RegisterType<DetailPage>();
            builder.RegisterType<SearchListPage>();
            builder.RegisterType<TabPage>();

            // current page resolver
            builder.RegisterInstance<Func<Page>>(Instance);
        }

        private static INetworkService Instance(Priority priority)
        {
            Func<HttpMessageHandler, INetworkService> createClient = messageHandler =>
            {
                // service registration
                var networkServiceSettings = new RefitSettings
                {
                    JsonSerializerSettings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }
                };

                var client = new HttpClient(messageHandler)
                {
                    BaseAddress = new Uri("http://vmkrcmar21.informatik.tu-muenchen.de/")
                };

                return RestService.For<INetworkService>(client, networkServiceSettings);
            };

            return
                new SafeNetworkService(
                    createClient(new RateLimitedHttpMessageHandler(new NativeMessageHandler(), priority)));
        }

        private static Page Instance()
        {
            var masterDetailPage = Application.Current.MainPage as MasterDetailPage;
            if (masterDetailPage == null)
            {
                return Application.Current.MainPage;
            }
            var navigationPage = masterDetailPage.Detail as NavigationPage;
            return navigationPage != null ? navigationPage.CurrentPage : masterDetailPage.Detail;
        }
    }
}
