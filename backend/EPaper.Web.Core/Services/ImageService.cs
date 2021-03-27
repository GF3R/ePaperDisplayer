using EPaper.Web.Core.Models;
using EPaper.Web.Core.Utility;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using EPaper.Web.Core.Models.Configurations;

namespace EPaper.Web.Core.Services
{
    public class ImageService : IImageService
    {
        private readonly TypeCodeConfiguration _configuration;
        private readonly Image _baseImage = Image.FromFile("Ressources/Base.png");

        public ImageService(TypeCodeConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Image CreateImageFromWeather(WeatherForecast weather)
        {
            var fontFamily = new FontFamily("Arial");
            var font = new Font(
                fontFamily,
                18,
                FontStyle.Regular,
                GraphicsUnit.Pixel);
            var small = new Font(
                fontFamily,
                12,
                FontStyle.Regular,
                GraphicsUnit.Pixel);

            var weatherInOneHour = weather.InOneHoursWeather;
            var tomorrowsWeather = weather.TomorrowsWeather;

            DateTime nowInTimeZone = DateTime.Now;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                nowInTimeZone = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(TimeZone.CurrentTimeZone.StandardName));
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                nowInTimeZone = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Europe/Zurich"));
            }

            using (Graphics grfx = Graphics.FromImage(_baseImage))
            {
                grfx.DrawImage(DrawText($"{weatherInOneHour.DateTime:ddd HH:mm}", font, Color.Black, Color.White), 35, 30);
                grfx.DrawImage(GetImageFromWeatherIconUrl(weatherInOneHour.ImageUrl), 10, 50);
                grfx.DrawImage(DrawText(weatherInOneHour.CurrTempAsString, font, Color.Black, Color.White), 60, 245);
                grfx.DrawImage(DrawText(weather.Now.CurrTempAsString, small, Color.Black, Color.White), 30, 275);
                grfx.DrawImage(DrawText(weather.InTwoHoursWeather.CurrTempAsString, small, Color.Black, Color.White), 100, 275);

                grfx.DrawImage(DrawText($"{tomorrowsWeather.DateTime:ddd dd.MM}", font, Color.Black, Color.White), 250, 30);
                grfx.DrawImage(GetImageFromWeatherIconUrl(tomorrowsWeather.ImageUrl), 210, 50);
                grfx.DrawImage(DrawText(tomorrowsWeather.MinTempAsString, font, Color.Black, Color.White), 250, 275);
                grfx.DrawImage(DrawText(tomorrowsWeather.MaxTempAsString, font, Color.Black, Color.White), 290, 245);

                grfx.DrawImage(DrawText(nowInTimeZone.ToString("HH:mm"), small, Color.Black, Color.White), 350, 280);

            }
            return _baseImage;
        }

        public Image GetImageFromWeatherIconUrl(string iconUrl, int width = 180, int height = 180)
        {
            if (this._configuration.FromFileSystem)
            {
                return Image.FromFile(iconUrl).ResizeImage(width, height);
            }
            else
            {
                using var webClient = new WebClient();
                using var ms = new MemoryStream(webClient.DownloadData(iconUrl));
                return ReplaceWwithBColor(Image.FromStream(ms).ResizeImage(width, height));
            }

        }

        private Image DrawText(String text, Font font, Color textColor, Color backColor)
        {
            //first, create a dummy bitmap just to get a graphics object
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);

            //measure the string to see how big the image needs to be
            SizeF textSize = drawing.MeasureString(text, font);

            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            //create a new image of the right size
            img = new Bitmap((int)textSize.Width, (int)textSize.Height);

            drawing = Graphics.FromImage(img);

            //paint the background
            drawing.Clear(backColor);

            //create a brush for the text
            Brush textBrush = new SolidBrush(textColor);

            drawing.DrawString(text, font, textBrush, 0, 0);

            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();

            return img;

        }

        private static Bitmap ReplaceWwithBColor(Image image)
        {
            Color black = Color.Black; //Your desired colour
            Color white = Color.White;

            Bitmap bmp = new Bitmap(image);
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    Color gotColor = bmp.GetPixel(x, y);
                    if (gotColor.GetBrightness() >= 0.99)
                    {
                        gotColor = Color.FromArgb(black.R, black.G, black.B);
                    }
                    else if (gotColor.GetBrightness() > 0.85 && gotColor.GetBrightness() < 0.98)
                    {
                        gotColor = Color.FromArgb(white.R, white.G, white.B);

                    }

                    bmp.SetPixel(x, y, gotColor);
                }
            }

            return bmp;
        }
    }
}
