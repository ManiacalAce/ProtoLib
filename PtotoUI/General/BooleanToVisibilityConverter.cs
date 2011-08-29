using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System;
using System.Windows;
using System.Windows.Data;

namespace ProtoUI.General
{
	/// <summary>
	/// Got from http://stackoverflow.com/questions/534575/how-do-i-invert-booleantovisibilityconverter
	/// Allows inverted conversions.
	/// </summary>
	public sealed class BooleanToVisibilityConverter : IValueConverter
	{
	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
	        var flag = false;
	        if (value is bool)
	        {
	            flag = (bool)value;
	        }
	        else if (value is bool?)
	        {
	            var nullable = (bool?)value;
	            flag = nullable.GetValueOrDefault();
	        }
	        if (parameter != null)
	        {
	            if (bool.Parse((string)parameter))
	            {
	                flag = !flag;
	            }
	        }
	        if (flag)
	        {
	            return Visibility.Visible;
	        }
	        else
	        {
	            return Visibility.Collapsed;
	        }
	    }
	
	    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	    {
	        var back = ((value is Visibility) && (((Visibility)value) == Visibility.Visible));
	        if (parameter != null)
	        {
	            if ((bool)parameter)
	            {
	                back = !back;
	            }
	        }
	        return back;
	    }
	}

}
