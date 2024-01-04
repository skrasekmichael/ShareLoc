using System.Globalization;

namespace ShareLoc.Client.App.Converters;

public sealed class FormatDistanceConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is double distance)
		{
			if (distance > 1000)
			{
				distance /= 1000;
				return $"{distance:F2} km";
			}
			else
			{
				return $"{distance:F2} m";
			}
		}

		return value;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
