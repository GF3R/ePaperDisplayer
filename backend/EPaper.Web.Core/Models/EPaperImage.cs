using ImageMagick;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace EPaper.Web.Core.Models
{
    public class EPaperImage
    {

        public EPaperType EPaperType { get; set; } = EPaperType.Horizontal1Bit;

        public byte[] Bytes { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public int WhiteToBlackThrehshold { get; set; } = 128;

        public int NumberOfBytes => Bytes.Length;

        public string Base64String => GetBase64String();

        public string EPaperBytesAsString => GetByteArrayString();

        public EPaperImage(string url, int width, int height, int whiteToBlackThrehshold) : this(url, width, height)
        {
            this.WhiteToBlackThrehshold = whiteToBlackThrehshold;
        }

        public EPaperImage(string url, int width, int height)
        {
            this.Width = width;
            this.Height = height;

            using var webClient = new WebClient();
            using var ms = new MemoryStream(webClient.DownloadData(url));
            var image = Image.FromStream(ms);
            Setup(image);

        }

        public EPaperImage(Image image, int width, int height)
        {
            this.Width = width;
            this.Height = height;
            Setup(image);
        }

        public byte[] GetEPaperBytes()
        {
            using (MagickImage magickImage = new MagickImage(this.Bytes))
            {
                magickImage.Format = MagickFormat.Bmp;
                var imagePixels = magickImage.GetPixels().ToByteArray(PixelMapping.RGBA);
                switch (this.EPaperType)
                {
                    case EPaperType.Vertical1Bit:
                        return Vertical1Bit(imagePixels);
                    case EPaperType.Horizontal1Bit:
                        return Horizontal1Bit(imagePixels);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void Setup(Image image)
        {
            using var newStream = new MemoryStream();
            {
                Image newImage = new Bitmap(image, new Size(this.Width, this.Height));
                newImage.Save(newStream, ImageFormat.Bmp);
                this.Bytes = newStream.ToArray();
            }
        }

        private string GetBase64String()
        {
            // Convert byte[] to Base64 String
            return Convert.ToBase64String(this.Bytes);
        }

        private string GetByteArrayString()
        {
            return BitConverter.ToString(this.GetEPaperBytes());
        }

        private byte[] Horizontal1Bit(byte[] data)
        {
            var returnedBytes = new List<byte>();
            var byteIndex = 7;
            double number = 0;

            // format is RGBA, so move 4 steps per pixel
            for (var index = 0; index < data.Length; index += 4)
            {
                // Get the average of the RGB (we ignore A)
                var avg = (data[index] + data[index + 1] + data[index + 2]) / 3;

                if (avg > WhiteToBlackThrehshold)
                {
                    number += Math.Pow(2, byteIndex);
                }

                byteIndex--;

                // if this was the last pixel of a row or the last pixel of the
                // image, fill up the rest of our byte with zeros so it always contains 8 bits
                if ((index != 0 && (((index / 4) + 1) % (this.Width)) == 0) || (index == data.Length - 4))
                {
                    byteIndex = -1;
                }

                if (byteIndex < 0)
                {
                    var numAsInt = Convert.ToInt32(number);
                    returnedBytes.Add(Convert.ToByte(numAsInt));
                    number = 0;
                    byteIndex = 7;
                }
            }
            return returnedBytes.ToArray();
        }

        private byte[] Vertical1Bit(byte[] data)
        {
            var screenHeight = Math.Floor((double)this.Height / 8);
            var returnedBytes = new List<byte>();
            for (var p = 0; p < screenHeight; p++)
            {
                for (var x = 0; x < this.Width; x++)
                {
                    var byteIndex = 7;
                    double number = 0;

                    for (var y = 7; y >= 0; y--)
                    {

                        int index = ((p * 8) + y) * (this.Width * 4) + x * 4;
                        if (index > data.Length)
                        {
                            throw new IndexOutOfRangeException($"Index: {index} is higher than the data length: {data.Length}");
                        }
                        var avg = (data[index] + data[index + 1] + data[index + 2]) / 3;
                        if (avg > this.WhiteToBlackThrehshold)
                        {
                            number += Math.Pow(2, byteIndex);
                        }
                        byteIndex--;
                    }
                    var numAsInt = Convert.ToInt32(number);
                    returnedBytes.Add(Convert.ToByte(numAsInt));
                }
            }
            return returnedBytes.ToArray();
        }
    }

    public enum EPaperType
    {
        Vertical1Bit,
        Horizontal1Bit
    }
}
