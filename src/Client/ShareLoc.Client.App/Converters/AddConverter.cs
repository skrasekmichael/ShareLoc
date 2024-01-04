using System.Globalization;

namespace ShareLoc.Client.App.Converters;

public sealed class AddConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is double originalValue && parameter is string parameterString)
		{
			if (double.TryParse(parameterString, out var bias))
			{
				return originalValue + bias;
			}
		}

		return value;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
