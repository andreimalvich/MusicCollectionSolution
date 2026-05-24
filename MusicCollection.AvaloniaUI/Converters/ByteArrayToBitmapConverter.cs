using System;
using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace MusicCollection.AvaloniaUI.Converters;

internal class ByteArrayToBitmapConverter : IValueConverter
{
    // Системный путь к картинке-заглушке, если обложки нет в БД
    private static Bitmap? _defaultAsset;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Если пришли валидные байты с сервера — превращаем их в Bitmap
        if (value is byte[] bytes && bytes.Length > 0)
        {
            try
            {
                using var stream = new MemoryStream(bytes);
                return new Bitmap(stream);
            }
            catch
            {
                // Если массив байт поврежден, возврат дефолтной загл
                return GetDefaultAsset();
            }
        }

        // Если картинки в базе нет (null) — возвращаем заглушку
        return GetDefaultAsset();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private static Bitmap GetDefaultAsset()
    {
        if (_defaultAsset == null)
        {
            // Загружаем встроенную заглушку из ресурсов
            var assetUri = new Uri("avares://MusicCollection.AvaloniaUI/Assets/no_cover.png");
            _defaultAsset = new Bitmap(AssetLoader.Open(assetUri));
        }

        return _defaultAsset;
    }
}
