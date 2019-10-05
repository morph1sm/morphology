using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Morphology
{
    public class DistinctConverter : IValueConverter
    {
        public object Convert(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<Morph> morphs = (value as IEnumerable<object>).Cast<Morph>().ToList();
            List<string> regions = morphs.Select(m => m.Region).ToList();
            //if (regions == null)
            //    return null;
            return regions.Cast<object>().Distinct();
        }

        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
