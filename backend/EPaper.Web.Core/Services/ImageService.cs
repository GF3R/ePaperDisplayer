using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using EPaper.Web.Core.Models;
using EPaper.Web.Core.Utility;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace EPaper.Web.Core.Services
{
    public class ImageService
    {
        private readonly Image _baseImage = Image.FromFile("Ressources/Base.png");

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
                grfx.DrawImage(DrawText($"{weatherInOneHour.DateTime:ddd HH:mm}", font, Color.Black, Color.White), 35, 10);
                grfx.DrawImage(getImageFromWeatherIconId(weatherInOneHour.ImageUrl), 0, 50);
                grfx.DrawImage(DrawText(weatherInOneHour.CurrTempAsString, font, Color.Black, Color.White), 60, 220);
                grfx.DrawImage(DrawText(weather.Now.CurrTempAsString, small, Color.Black, Color.White), 30, 250);
                grfx.DrawImage(DrawText(weather.InTwoHoursWeather.CurrTempAsString, small, Color.Black, Color.White), 100, 250);

                grfx.DrawImage(DrawText($"{tomorrowsWeather.DateTime:ddd dd.MM}", font, Color.Black, Color.White), 250, 10);
                grfx.DrawImage(getImageFromWeatherIconId(tomorrowsWeather.ImageUrl), 200, 50);
                grfx.DrawImage(DrawText(tomorrowsWeather.MinTempAsString, font, Color.Black, Color.White), 250, 250);
                grfx.DrawImage(DrawText(tomorrowsWeather.MaxTempAsString, font, Color.Black, Color.White), 290, 220);

                grfx.DrawImage(DrawText(nowInTimeZone.ToString("HH:mm"), small, Color.Black, Color.White), 350, 280);

            }
            return _baseImage;
        }

        private Image getImageFromWeatherIconId(string iconUrl)
        {
            using var webClient = new WebClient();
            using var ms = new MemoryStream(webClient.DownloadData(iconUrl));
            return Image.FromStream(ms).ResizeImage(180, 180);
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
    }
}
