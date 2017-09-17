using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Randao.Web
{
	public static class HttpRequestBaseExtensions
	{
		public static string GetIp(this HttpRequestBase request)
		{
			if (request.ServerVariables["HTTP_VIA"] != null)
			{
				return request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(new char[] { ',' })[0];
			}
			else
			{
				return request.ServerVariables["REMOTE_ADDR"];
			}
		}

		public static string GetIp(this HttpRequest request)
		{
			if (request.ServerVariables["HTTP_VIA"] != null)
			{
				return request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(new char[] { ',' })[0];
			}
			else
			{
				return request.ServerVariables["REMOTE_ADDR"];
			}
		}
	}
}