﻿using System;
using Windows.UI.Popups;

namespace Trains.Services.Tools
{
    public static class ToolHelper
    {
        private const string Everyday = "everyday";

        public static async void ShowMessageBox(string message)
        {
            var dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }
        public static string GetDate(DateTimeOffset datum, string selectedVariantOfSearch = null)
        {
            if (selectedVariantOfSearch == Everyday) return Everyday;
            return datum.Date.Year + "-" + datum.Date.Month + "-" + datum.Date.Day;
        }
    }
}
