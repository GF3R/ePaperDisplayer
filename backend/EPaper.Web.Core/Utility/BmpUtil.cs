using System;
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
    }
}
