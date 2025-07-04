using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace NirvanaMAUI.Converters;

public class GamepadConnectionTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool connected)
            return connected ? "Gamepad conectado" : "Gamepad não detectado";
        return "Gamepad não detectado";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}
