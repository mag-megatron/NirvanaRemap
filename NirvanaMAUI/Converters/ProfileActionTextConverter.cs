using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using NirvanaMAUI.Models;
using System.Collections.ObjectModel;

namespace NirvanaMAUI.Converters
{
    public class ProfileActionTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Profile profile)
                return "";

            var collectionView = parameter as CollectionView;
            if (collectionView?.ItemsSource is ObservableCollection<Profile> profiles)
            {
                // Se for o primeiro da lista ("Padrão"), botão deve ser "+"
                return profile == profiles.FirstOrDefault() ? "+" : "–";
            }

            return profile.Name == "Padrão" ? "+" : "–";
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
