using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skillmap.Repositories
{
    public class ImageRepository : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var base64Image = value as string;
            if (string.IsNullOrEmpty(base64Image))
                return null;

            try
            {
                byte[] imageBytes = System.Convert.FromBase64String(base64Image);
                return ImageSource.FromStream(() => new MemoryStream(imageBytes));
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
