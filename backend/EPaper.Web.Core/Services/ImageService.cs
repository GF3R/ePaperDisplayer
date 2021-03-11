using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using EPaper.Web.Core.Models;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace EPaper.Web.Core.Services
{
    public class ImageService
    {
        private readonly string _baseUrl = "http://openweathermap.org/img/wn/{0}@4x.png";

        private readonly Image _baseImage = Image.FromFile("Ressources/Base.png");

        public Image CreateImageFromWeather(WeatherResponse weather)
        {
            var fontFamily = new FontFamily("Arial");
            var font = new Font(
                fontFamily,
                26,
                FontStyle.Regular,
                GraphicsUnit.Pixel);
            var small = new Font(
                fontFamily,
                12,
                FontStyle.Regular,
                GraphicsUnit.Pixel);

            var todaysWeather = weather.GetWeatherOfToday();
            var tomorrowsWeather = weather.GetWeatherOfTomorrow();
            var nowInTimeZone = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(TimeZone.CurrentTimeZone.StandardName));
            using (Graphics grfx = Graphics.FromImage(_baseImage))
            {
                var today = DateTime.Today;
                grfx.DrawImage(DrawText($"{today:dd.MM} {today:ddd}", font, Color.Black, Color.White), 30, 10);
                grfx.DrawImage(getImageFromWeatherIconId(todaysWeather.weather.First().icon), 0, 50);
                grfx.DrawImage(DrawText(todaysWeather.temp.DayAsCelsiusString(), font, Color.Black, Color.White), 30, 250);

                var tomorrow = today.AddDays(1);

                grfx.DrawImage(DrawText($"{tomorrow:dd.MM} {tomorrow:ddd}", font, Color.Black, Color.White), 230, 10);
                grfx.DrawImage(getImageFromWeatherIconId(tomorrowsWeather.weather.First().icon), 200, 50);
                grfx.DrawImage(DrawText(tomorrowsWeather.temp.DayAsCelsiusString(), font, Color.Black, Color.White), 230, 250);
                grfx.DrawImage(DrawText(nowInTimeZone.ToString("HH:mm"), small, Color.Black, Color.White), 350, 280);

            }
            return _baseImage;
        }

        private Image getImageFromWeatherIconId(string iconId)
        {
            using var webClient = new WebClient();
            var url = string.Format(_baseUrl, iconId);
            using var ms = new MemoryStream(webClient.DownloadData(url));
            return Image.FromStream(ms);
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
