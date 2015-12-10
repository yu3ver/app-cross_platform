﻿using Integreat.Shared.ViewModels;
using Integreat.Shared.Views;
using Xamarin.Forms;
using Page = Integreat.Models.Page;

namespace Integreat.Shared.Pages
{
    public partial class InformationOverviewPage : ContentPage
    {
        public PagesViewModel ViewModel
        {
            get { return BindingContext as PagesViewModel; }
        }

        public InformationOverviewPage()
        {
            InitializeComponent();
            BindingContext = new PagesViewModel();
            
            listView.ItemTapped += (sender, args) =>
            {
                if (listView.SelectedItem == null) { 
                    return;
                }
                var page = listView.SelectedItem as Page;
                Navigation.PushAsync(new WebsiteView(page));
                listView.SelectedItem = null;
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (ViewModel == null || !ViewModel.CanLoadMore || ViewModel.IsBusy || ViewModel.Pages.Count > 0) { 
                return;
            }

            ViewModel.LoadPagesCommand.Execute(null);
        }
    }
}