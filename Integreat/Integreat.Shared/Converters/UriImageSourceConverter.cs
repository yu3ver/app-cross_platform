﻿using System;
using System.Security;
using Xamarin.Forms;

namespace Integreat.Shared.Converters
{
    /// <summary>
    /// converts a string to an uriimagesource
    /// </summary>
    public class UriImageSourceConverter : IValueConverter
	{
		[SecurityCritical]
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (!(value is string) || "".Equals(value)) return null;

			var image = (string)value;

			try
			{
				return new UriImageSource
				{
					Uri = new Uri(image),
					CachingEnabled = true,
					CacheValidity = new TimeSpan(1, 0, 0, 0)
				};
			}
			catch (Exception)
			{
				return image;
			}
		}
		[SecurityCritical]
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
	}
}
