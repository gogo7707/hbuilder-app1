using System;
using System.Runtime.Serialization;

namespace Randao.DataContracts
{
	/// <summary>
	/// ÊÕ²Ø¼ÇÂ¼
	/// </summary>
	[Serializable, DataContract(Name = " article_collect")]
    public class ArticleCollectDataContract
    {
		[DataMember(Name = "collect_id")]
		public Int64 CollectId { get; set; }

		[DataMember(Name = "article_id")]
		public Int64 ArticleID { get; set; }

		[DataMember(Name = "category_id")]
		public int CategoryId
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.TableName))
				{
					return -1;
				}
				return Convert.ToInt32(this.TableName.ToLower().Replace("sa_articlecontent", string.Empty));
			}
		}
		[DataMember(Name = "title")]
		public String Title { get; set; }

		[DataMember(Name = "user_key_id")]
		public Int64 UserKeyId { get; set; }

		[DataMember(Name = "author")]
		public String Author { get; set; }

		[DataMember(Name = "author_user_key_id")]
		public long AuthorUserKeyId { get; set; }

		[DataMember(Name = "face_img")]
		public string FaceImg { get; set; }

		[DataMember(Name = "collect_time")]
        public System.DateTime CollectTime { get; set; }

        public String TableName { get; set; }
    }
}
