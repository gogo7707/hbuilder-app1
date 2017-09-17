using System;
using System.Runtime.Serialization;

namespace Randao.DataContracts
{
	[Serializable, DataContract(Name = "access_token")]
	public class OauthToken
	{
		[DataMember(Name = "access_token")]
		public string AccessToken { get; set; }
		/// <summary>
		/// AppID
		/// </summary>
		[DataMember(Name = "client_id")]
		public int ClientID { get; set; }
		/// <summary>
		/// 用户id
		/// </summary>
		[DataMember(Name = "user_key_id")]
		public long UserKeyId { get; set; }

		/// <summary>
		/// 有效期
		/// </summary>
		[DataMember(Name = "expires")]
		public Int64 Expires { get; set; }
		/// <summary>
		/// 权限
		/// </summary>
		[DataMember(Name = "scope")]
		public string Scope { get; set; }

		/// <summary>
		/// 刷新token
		/// </summary>
		[DataMember(Name = "refresh_token")]
		public string RefreshToken { get; set; }

		public string ClientIp { get; set; }

		[DataMember(Name = "user_info")]
		public TdMemberDataContract User { get; set; }
	}
}
