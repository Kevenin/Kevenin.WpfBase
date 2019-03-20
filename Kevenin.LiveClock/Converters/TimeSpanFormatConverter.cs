using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Kevenin.LiveClock.Converters
{
	public class TimeSpanFormatConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			TimeSpan time = (TimeSpan)values[0];
			string format = values[1].ToString();
			format = format.Replace(":", "\\:").Replace(".", "\\.");
			return time.ToString(@format);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}