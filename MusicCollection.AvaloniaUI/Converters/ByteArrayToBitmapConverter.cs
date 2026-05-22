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
        // 1. Если пришли валидные байты из MS SQL Server — превращаем их в Bitmap
        if (value is byte[] bytes && bytes.Length > 0)
        {
            try
            {
                using var stream = new MemoryStream(bytes);
                return new Bitmap(stream);
            }
            catch
            {
                // Если массив байт поврежден, проваливаемся в дефолтную заглушку
                return GetDefaultAsset();
            }
        }

        // 2. Если картинки в базе нет (null) — возвращаем красивую заглушку
        return GetDefaultAsset();
    }


    //public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    //{
    //    // Улучшенная, всеядная проверка: пытаемся привести к IEnumerable или byte[]
    //    if (value is byte[] bytes)
    //    {
    //        if (bytes.Length == 0) return GetDefaultAsset();

    //        try
    //        {
    //            using var stream = new MemoryStream(bytes);
    //            return new Bitmap(stream);
    //        }
    //        catch
    //        {
    //            return GetDefaultAsset();
    //        }
    //    }

    //    // Если база вернула пустые данные или null
    //    return GetDefaultAsset();
    //}




    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private static Bitmap GetDefaultAsset()
    {
        if (_defaultAsset == null)
        {
            // Загружаем встроенную заглушку из ресурсов самого AvaloniaUI проекта.
            // Убедитесь, что добавили файл "no_cover.png" в папку Assets вашего проекта!
            var assetUri = new Uri("avares://MusicCollection.AvaloniaUI/Assets/no_cover.png");
            _defaultAsset = new Bitmap(AssetLoader.Open(assetUri));
        }
        return _defaultAsset;
    }
}
