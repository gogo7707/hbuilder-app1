using System;
using System.Web;

namespace Randao.Core.FormsAuth
{
	/// <summary>
	/// 加密写用户cookie
	/// </summary>
	internal class FormsAuthenticationWrapper : Singleton<FormsAuthenticationWrapper>
	{
		private FormsAuthenticationWrapper() { }

		private static readonly string currDomain = "randao.com";
		public void SetAuthCookie(string userKeyId, bool createPersistentCookie)
		{
			string skey = Guid.NewGuid().ToString("N").Substring(0, 20);

			var authCookie = new HttpCookie("authKey")
			{
				Value = StringExtensions.TripleDESC("RANDAO85D136D1FDA1FDS33H51IN0DC6F441W1W4VN8ZNE1LU9N@app.com", userKeyId + "|" + skey),
				Domain = currDomain
			};

			if (createPersistentCookie)
			{
				authCookie.Expires = DateTime.Now.AddDays(30);
			}
			HttpContext.Current.Response.Cookies.Add(authCookie);

			var uinCookie = new HttpCookie("userKeyId")
			{
				Value = userKeyId,
				Domain = currDomain
			};
			if (createPersistentCookie)
			{
				uinCookie.Expires = DateTime.Now.AddDays(30);
			}
			HttpContext.Current.Response.Cookies.Add(uinCookie);

			var sKey = new HttpCookie("skey")
			{
				Value = skey,
				Domain = currDomain
			};
			if (createPersistentCookie)
			{
				sKey.Expires = DateTime.Now.AddDays(30);
			}
			HttpContext.Current.Response.Cookies.Add(sKey);
		}

		public void SignOut()
		{
			throw new NotImplementedException();
		}
	}
}
