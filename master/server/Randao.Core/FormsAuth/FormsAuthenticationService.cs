using System;
using System.Linq;

namespace Randao.Core.FormsAuth
{
	/// <summary>
	/// 解密cookie中的用户信息
	/// </summary>
	internal class FormsAuthenticationService
	{
		public FormsAuthenticationService()
		{
			userKeyId = GetAuthenticatedUserKeyId();
		}

		public long userKeyId;

		private long GetAuthenticatedUserKeyId()
		{
			var httpContext = System.Web.HttpContext.Current;

			var authCookie = httpContext.Request.Cookies["authKey"];
			var skeyCookie = httpContext.Request.Cookies["skey"];
			if (null == authCookie || null == skeyCookie)
			{
				return 0;
			}

			var encryptedTicket = authCookie.Value;
			var skeyValue = skeyCookie.Value;

			if (!string.IsNullOrEmpty(encryptedTicket))
			{
				var authTicket = encryptedTicket.Decrypt<System.Security.Cryptography.TripleDESCryptoServiceProvider>("RANDAO85D136D1FDA1FDS33H51IN0DC6F441W1W4VN8ZNE1LU9N@app.com");
				if (!string.IsNullOrEmpty(skeyValue) && authTicket.Split('|').Any(t => t.Equals(skeyValue)))
				{
					return Convert.ToInt32(authTicket.Split('|').First());
				}
			}

			return 0;
		}
	}
}
