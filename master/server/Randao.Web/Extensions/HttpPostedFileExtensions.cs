using System;
using System.Web;

namespace Randao.Core.Extensions
{
	public static class HttpPostedFileExtensions
	{
		public const int MinSize = 0;
		public const int MaxSize = 1024 * 1024 * 1;
		public const string ExtFormat = ".jpg|.gif|.png";
		private static string _imgagePath = string.Empty;

		public static string ImagePath
		{
			get
			{
				if (string.IsNullOrEmpty(_imgagePath))
				{
					_imgagePath = HttpContext.Current.Server.MapPath("~");
				}
				return _imgagePath;
			}
		}

		/// <summary>
		/// 检验文件类型
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public static bool CheckType(this HttpPostedFileBase file)
		{
			string ext = file.GetExtName();
			return ExtFormat.IndexOf(ext.ToLower()) < 0;
		}

		/// <summary>
		/// 获取后缀
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public static string GetExtName(this HttpPostedFileBase file)
		{
			return file.FileName.Substring(file.FileName.LastIndexOf('.'));
		}

		private static string SaveThumbnailImage(this HttpPostedFileBase file, string filename, string ext, int width, int height, Thumbnail ThumbnailMode)
		{
			string tfilename = string.Format("{0}_{1}x{2}", filename, width, height);
			ThumbnailImage.GetThumbnailImage(file, string.Format("{0}{1}", tfilename, ext), width, height, ThumbnailMode);
			return filename;
		}

		public static ImageSize SaveAvatarPhoto(this HttpPostedFileBase file, string dir)
		{
			string currPath = ImagePath + dir;
			if (!System.IO.Directory.Exists(currPath))
			{
				System.IO.Directory.CreateDirectory(currPath);
			}

			DateTime dt = System.DateTime.Now;
			Random ra = new Random();
			string RanStr = string.Empty;
			for (int i = 0; i <= 1; i++)
			{
				int abc = ra.Next(0, 9);
				RanStr += abc.ToString();
			}
			string getName = file.FileName.Substring(file.FileName.IndexOf("."));
			string filename = dt.ToString("yyyyMMddHHmmss") + dt.Millisecond.ToString() + RanStr + getName;

			ImageSize size = new ImageSize();
			file.SaveAs(currPath + "\\" + filename);
			file.SaveThumbnailImage(currPath + "\\" + filename, ".jpg", 600, 300, Thumbnail.Cut);
			file.SaveThumbnailImage(currPath + "\\" + filename, ".jpg", 150, 75, Thumbnail.Cut);
			size.name = dir + "/" + filename;
			return size;
		}
	}
}
