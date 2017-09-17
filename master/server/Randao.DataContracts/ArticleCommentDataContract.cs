using System;
using System.Runtime.Serialization;

namespace Randao.DataContracts
{
	/// <summary>
	/// ∆¿¬€ µÃÂ
	/// </summary>
	[Serializable, DataContract(Name = " article_comment")]
	public class ArticleCommentDataContract
	{
		[DataMember(Name = "post_time")]
		public System.DateTime PostTime { get; set; }

		public System.DateTime UpdateTime { get; set; }

		[DataMember(Name = "comment_id")]
		public Int64 CommentID { get; set; }

		[DataMember(Name = "article_id")]
		public Int64 ArticleID { get; set; }

		[DataMember(Name = "flower_count")]
		public Int64 FlowerCount { get; set; }

		[DataMember(Name = "egg_count")]
		public Int64 EggCount { get; set; }

		[DataMember(Name = "warn_count")]
		public Int64 WarnCount { get; set; }

		[DataMember(Name = "user_key_id")]
		public Int64 UserKeyId { get; set; }

		[DataMember(Name = "nick_name")]
		public string NickName { get; set; }

		[DataMember(Name = "face_img")]
		public string FaceImg { get; set; }

		public String TableName { get; set; }

		[DataMember(Name = "comment_content")]
		public String CommentContent { get; set; }
	}
}
	