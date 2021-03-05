using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using EPaper.Web.Core.Models;
using ImageMagick;

namespace EPaper.Web.Core.Utility
{
    public static class BmpUtil
    {
        public static void ConvertToGrayScale(this Bitmap bitmap)
        {
            int x, y;
            for (x = 0; x < bitmap.Width; x++)
            {
                for (y = 0; y < bitmap.Height; y++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    Color newColor = Color.FromArgb(pixelColor.R, 0, 0);
                    bitmap.SetPixel(x, y, newColor); // Now greyscale
                }
            }
        }

        public static void ReducePixelDensitiy(this Bitmap bitmap)
        {
            int x, y;
            for (x = 0; x < bitmap.Width; x++)
            {
                for (y = 0; y < bitmap.Height; y++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    Color newColor = Color.FromArgb(pixelColor.R, 0, 0);
                    bitmap.SetPixel(x, y, newColor); // Now greyscale
                }
            }
        }

        public static void CopyToBitMap(this Bitmap bitmap, Bitmap bitmapToCopy)
        {
            BitmapData bmd = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite, bitmap.PixelFormat);
            bitmap.UnlockBits(bmd);
            int x, y;
            for (x = 0; x < bitmapToCopy.Width; x++)
            {
                for (y = 0; y < bitmapToCopy.Height; y++)
                {
                    Color pixelColor = bitmapToCopy.GetPixel(x, y);
                    Color newColor = Color.FromArgb(pixelColor.R, 0, 0);
                    bitmap.SetPixel(x, y, newColor); // Now greyscale
                }
            }
        }

        public static void CopyToBitByteArray(this Bitmap bitmap, int stride,  byte[] byteArray)
        {
            BitmapData bmd = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite, bitmap.PixelFormat);
            bitmap.UnlockBits(bmd);
            int x, y;
            for (x = 0; x < bitmap.Width; x++)
            {
                for (y = 0; y < bitmap.Height; y++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    Color newColor = Color.FromArgb(pixelColor.R, 0, 0);
                    byteArray[stride * y + x] = pixelColor.R; // Now greyscale
                    byteArray[stride * y + x +1] = pixelColor.G; // Now greyscale
                    byteArray[stride * y + x + 2] = pixelColor.B; // Now greyscale

                }
            }
        }

        public static ImageWrapper ByteArrayBmpToBase64String(byte[] bytes)
        {
            using (MagickImage image = new MagickImage(bytes))
            {
                var s = new ImageWrapper();
                s.Base64 = image.ToBase64();
                s.NumberOfBytes = bytes.Length;
                s.FileSize = BitConverter.ToInt32(bytes.Skip(2).Take(4).ToArray());
                s.CreatorByte = BitConverter.ToInt32(bytes.Skip(6).Take(4).ToArray());
                s.ImageOffset = BitConverter.ToInt32(bytes.Skip(10).Take(4).ToArray());
                s.HeaderSize = BitConverter.ToInt32(bytes.Skip(14).Take(4).ToArray());
                s.Width = BitConverter.ToInt32(bytes.Skip(18).Take(4).ToArray());
                s.Height = BitConverter.ToInt32(bytes.Skip(22).Take(4).ToArray());
                s.Planes = BitConverter.ToInt16(bytes.Skip(26).Take(2).ToArray());
                s.Depth = BitConverter.ToInt16(bytes.Skip(28).Take(2).ToArray());
                s.Format = BitConverter.ToInt32(bytes.Skip(30).Take(4).ToArray());
                return s;
            }
        }

        public static byte[] UrlToBitmapWithResolutions(string url, int width, int height)
        {
            using var webClient = new WebClient();
            var data = webClient.DownloadData(url);

            using var ms = new MemoryStream(data);
            using var stream = new MemoryStream();

            using (MagickImage image = new MagickImage(data))
            {
                image.Resize(width, height); // fit the image into the requested width and height. 
                image.Grayscale();
                //image.ColorType = ColorType.Palette;
                //image.Depth = 8;
                image.Format = MagickFormat.Bmp;
                //image.Quantize(new QuantizeSettings() { Colors = 256, DitherMethod = DitherMethod.No });
                image.SetCompression(CompressionMethod.NoCompression);
                image.Write(stream, MagickFormat.Bmp);
                //return stream.ToArray();
            }



            using var bitmap = new Bitmap(stream);
            using var newStream = new MemoryStream();
            {
                //Marshal.Copy(bytes, 0, eightData.Scan0, bytes.Length);
                //eightBitmap.CopyToBitMap(bitmap);
                var parameters = new EncoderParameters(1) {Param = {[0] = new EncoderParameter(Encoder.ColorDepth, 8L)}};
                var info = ImageCodecInfo.GetImageEncoders()
                    .FirstOrDefault(info => info.MimeType == "image/bmp");
                bitmap.Save(newStream, info, parameters);
                return newStream.ToArray();
            }
        }

        public static byte[] GetBytesFromUrl(string url, int width, int height)
        {
            using var webClient = new WebClient();
            var data = webClient.DownloadData(url);
            using var ms = new MemoryStream(data);

            Image newImage = new Bitmap(Image.FromStream(ms), new Size(width, height));
            using var newStream = new MemoryStream();
            newImage.Save(newStream, ImageFormat.Bmp);
            newStream.Seek(0, SeekOrigin.Begin);
            using (MagickImage image = new MagickImage(newStream))
            {
                image.Resize(width, height); // fit the image into the requested width and height. 
                image.Format = MagickFormat.Bmp;
                return Horizontal1Bit(image.GetPixels().ToByteArray(PixelMapping.RGBA), image.Width);
            }

        }


        public static byte[] Horizontal1Bit(byte[] data, int width, int threshold = 128)
        {
            var outputString = "";
            var outputIndex = 0;
            var returnedBytes = new List<byte>();
            var byteIndex = 7;
            double number = 0;

            // format is RGBA, so move 4 steps per pixel
            for (var index = 0; index < data.Length; index += 4)
            {
                // Get the average of the RGB (we ignore A)
                var avg = (data[index] + data[index + 1] + data[index + 2]) / 3;
                if (avg > threshold)
                {
                    number += Math.Pow(2, byteIndex);
                }
                byteIndex--;

                // if this was the last pixel of a row or the last pixel of the
                // image, fill up the rest of our byte with zeros so it always contains 8 bits
                if ((index != 0 && (((index / 4) + 1) % (width)) == 0) || (index == data.Length - 4))
                {
                    // for(var i=byteIndex;i>-1;i--){
                    // number += Math.pow(2, i);
                    // }
                    byteIndex = -1;
                }

                // When we have the complete 8 bits, combine them into a hex value
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

        public static byte[] Vertical1Bit(byte[] data, int width, int height, int threshold = 128)
        {
            double screenHeight = Math.Floor((double)height / 8);
            var returnedBytes = new List<byte>();
            for (var p = 0; p < screenHeight; p++)
            {
                for (var x = 0; x < width; x++)
                {
                    var byteIndex = 7;
                    double number = 0;

                    for (var y = 7; y >= 0; y--)
                    {
                        
                        int index = ((p * 8) + y) * (width * 4) + x * 4;
                        if (index > data.Length)
                        {
                            Debugger.Break();
                        }
                        var avg = (data[index] + data[index + 1] + data[index + 2]) / 3;
                        if (avg > threshold)
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
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }
}
