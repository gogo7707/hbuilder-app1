using System;
using System.Runtime.Serialization;

namespace Randao.DataContracts
{
	[Serializable, DataContract(Name = "refresh_token")]
	public class RefreshTokenDataContract
	{
		[DataMember(Name = "refresh_token")]
		public string RefreshToken { get; set; }
		/// <summary>
		/// AppID
		/// </summary>
		[DataMember(Name = "client_id")]
		public int ClientID { get; set; }
		/// <summary>
		/// 用户id
		/// </summary>
		[DataMember(Name = "user_key_id")]
		public long UserKeyID { get; set; }

		/// <summary>
		/// 有效期
		/// </summary>
		[DataMember(Name = "expires")]
		public Int64 Expires { get; set; }
	}
}
