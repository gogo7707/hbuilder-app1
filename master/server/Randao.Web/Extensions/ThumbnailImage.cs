using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;

namespace Randao.Core.Extensions
{
    public enum Thumbnail
    {
        /// <summary>
        /// 指定高宽缩放（可能变形）
        /// </summary>
        HW,
        /// <summary>
        /// 指定宽，高按比例   
        /// </summary>
        W,
        /// <summary>
        /// 指定高，宽按比例
        /// </summary>
        H,
        /// <summary>
        /// 指定高宽裁减（不变形）
        /// </summary>
        Cut,
        AutoWH,
        Auto
    }

    public class ImageSize
    {
        public float width { get; set; }
        public float height { get; set; }
        public string name { get; set; }
    }

    public class ThumbnailImage
    {
        public static ImageSize GetThumbnailImage(System.Web.HttpPostedFileBase postfile, string outname, float width, float height, Thumbnail Thumbnailmode)
        {
            return GetThumbnailImage(postfile.InputStream, outname, width, height, Thumbnailmode);
        }
        public static ImageSize GetThumbnailImage(Stream str, string outname, float width, float height, Thumbnail Thumbnailmode)
        {
            Image originalImage = Image.FromStream(str);
            return GetThumbnailImage(originalImage, outname, width, height, Thumbnailmode);
        }
        public static ImageSize GetThumbnailImage(string dir, string filename, string outname, float width, float height, Thumbnail Thumbnailmode)
        {
            Image originalImage = Image.FromFile(dir + filename);
            return GetThumbnailImage(originalImage, outname, width, height, Thumbnailmode);
        }
        public static ImageSize GetImageSzie(System.Web.HttpPostedFileBase postfile)
        {
            return GetImageSzie(postfile.InputStream);
        }
        public static ImageSize GetImageSzie(Stream str)
        {
            Image originalImage = Image.FromStream(str);
            return GetImageSzie(originalImage);
        }
        public static ImageSize GetImageSzie(Image originalImage)
        {
            return new ImageSize() { width = originalImage.Width, height = originalImage.Height };
        }

        public static ImageSize GetThumbnailImage(Image originalImage, string outname, float width, float height, Thumbnail Thumbnailmode)
        {

            float x = 0;
            float y = 0;
            float w = originalImage.Width;
            float h = originalImage.Height;

            #region A
            //if (originalImage.Width > width && originalImage.Height > height)
            //{
            //    if (originalImage.Width > originalImage.Height)
            //    {
            //        w = width;
            //        h = width * originalImage.Height / originalImage.Width;
            //        x = 0;
            //        y = (height - h) / 2;
            //        if (h > height)
            //        {
            //            h = height;
            //            w = height * originalImage.Width / originalImage.Height;
            //            x = (width - w) / 2;
            //            y = 0;
            //        }
            //    }
            //    else
            //    {
            //        h = height;
            //        w = height * (float)originalImage.Width / (float)originalImage.Height;
            //        x = (width - w) / 2;
            //        y = 0;
            //        if (w > width)
            //        {
            //            w = width;
            //            h = width * (float)originalImage.Height / (float)originalImage.Width;
            //            x = 0;
            //            y = (height - h) / 2;
            //        }
            //    }
            //}
            //else if ((float)originalImage.Width > width)
            //{
            //    w = width;
            //    h = width * (float)originalImage.Height / (float)originalImage.Width;
            //    x = 0;
            //    y = (height - h) / 2;
            //}
            //else if ((float)originalImage.Height > height)
            //{
            //    h = height;
            //    w = height * (float)originalImage.Width / (float)originalImage.Height;
            //    x =  (width - w) / 2;
            //    y = 0;
            //}
            //else
            //{
            //    w = (float)originalImage.Width;
            //    h = (float)originalImage.Height;
            //    x =  (width - w) / 2;
            //    y =  (height - h) / 2;
            //}
            //if (auto)
            //{
            //    x = 0;
            //    y = 0;
            //}
            #endregion

            switch (Thumbnailmode)
            {
                case Thumbnail.HW:
                    break;
                case Thumbnail.W:
                    height = originalImage.Height * width / originalImage.Width;
                    break;
                case Thumbnail.H:
                    width = originalImage.Width * height / originalImage.Height;
                    break;
                case Thumbnail.Cut:
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)width / (double)width)
                    {
                        h = originalImage.Height;
                        w = originalImage.Height * width / height;
                        y = 0;
                        x = (originalImage.Width - w) / 2;
                    }
                    else
                    {
                        w = originalImage.Width;
                        h = originalImage.Width * height / width;
                        x = 0;
                        y = (originalImage.Height - h) * 0.382f;
                    }
                    break;
                case Thumbnail.AutoWH:
                    if ((double)originalImage.Width / (double)originalImage.Height < (double)width / (double)width)
                    {
                        h = originalImage.Height;
                        w = originalImage.Height * width / height;
                        y = (originalImage.Height - h) / 2;
                        x = (originalImage.Width - w) / 2;
                    }
                    else
                    {
                        w = originalImage.Width;
                        h = originalImage.Width * height / width;
                        x = (originalImage.Width - w) / 2;
                        y = (originalImage.Height - h) / 2;
                    }
                    break;
                case Thumbnail.Auto:
                    if (originalImage.Width < width && originalImage.Height < height)
                    {
                        width = originalImage.Width;
                        height = originalImage.Height;
                    }
                    else if (originalImage.Width < width && originalImage.Height > height)
                    {
                        height = originalImage.Height * width / originalImage.Width;
                    }
                    else if (originalImage.Height < height && originalImage.Width > width)
                    {
                        width = originalImage.Width * height / originalImage.Height;
                    }
                    else
                    {
                        if (originalImage.Width > originalImage.Height)
                        {
                            height = originalImage.Height * width / originalImage.Width;
                        }
                        else
                        {
                            width = originalImage.Width * height / originalImage.Height;
                        }
                    }
                    break;
                default:
                    break;
            }
            Bitmap bm = new Bitmap((int)width, (int)height);
            Graphics g = Graphics.FromImage(bm);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.Clear(Color.White);
            //g.DrawImage(originalImage, new Rectangle((int)x, (int)y, (int)w, (int)h), 0, 0, originalImage.Width, originalImage.Height, GraphicsUnit.Pixel);    
            g.DrawImage(originalImage, new Rectangle(0, 0, (int)width, (int)height), new Rectangle((int)x, (int)y, (int)w, (int)h), GraphicsUnit.Pixel);
            long[] quality = new long[1];
            quality[0] = 100;
            System.Drawing.Imaging.EncoderParameters encoderParams = new System.Drawing.Imaging.EncoderParameters();
            System.Drawing.Imaging.EncoderParameter encoderParam = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            encoderParams.Param[0] = encoderParam;
            ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo jpegICI = null;

            string fd = outname.Substring(outname.LastIndexOf('.')).ToLower().TrimStart('.');
            fd = fd == "jpg" ? "JPEG" : fd == "gif" ? "GIF" : fd.ToUpper();

            for (int i = 0; i < arrayICI.Length; i++)
            {
                if (arrayICI[i].FormatDescription.Equals(fd))
                {
                    jpegICI = arrayICI[i];
                    break;
                }
            }
            if (jpegICI != null)
            {
                System.IO.File.Delete(outname);
                bm.Save(outname, jpegICI, encoderParams);
            }

            bm.Dispose();
            originalImage.Dispose();
            g.Dispose();
            return new ImageSize() { width = width, height = height };
        }
    }
}
