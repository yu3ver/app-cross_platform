﻿using System.ComponentModel;

namespace Integreat.Shared.Pages
{
	public partial class NavigationDrawerPage : BaseContentPage
	{
		public NavigationDrawerPage ()
		{
			InitializeComponent ();
		    ListViewMenu.Header = BindingContext;
            PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName.Equals("BindingContext"))
                {
                    ListViewMenu.Header = BindingContext;
                }
            };
        }
    }
}