using System;
using System.Runtime.Serialization;

namespace Randao.DataContracts
{
	/// <summary>
	/// ¹Ø×¢±í
	/// </summary>
	[Serializable, DataContract(Name = "article_attention")]
	public class ArticleAttentionDataContract
	{
		[DataMember(Name = "attention_time")]
		public DateTime AttentionTime { get; set; }

		[DataMember(Name = "Attention_id")]
		public Int64 AttentionId { get; set; }

		[DataMember(Name = "user_key_id")]
		public Int64 UserKeyId { get; set; }

		[DataMember(Name = "attention_user_key_id")]
		public Int64 AttentionUserKeyId { get; set; }

		[DataMember(Name = "attention_user_key_name")]
		public string AttentionUserKeyName { get; set; }

		[DataMember(Name = "attention_user_face_img")]
		public string AttentionUserFaceImg { get; set; }
	}
}
