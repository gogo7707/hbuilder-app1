using System;
using System.Runtime.Serialization;

namespace Randao.DataContracts
{
	/// <summary>
	/// 快捷登陆
	/// </summary>
	public class QuickLoginDataContract
	{
		public Guid UserId { get; set; }

		public string OpenId { get; set; }

		public string AccessToken { get; set; }

		public long ExpiredToken { get; set; }

		public DateTime CreateDate { get; set; }
	}
}
