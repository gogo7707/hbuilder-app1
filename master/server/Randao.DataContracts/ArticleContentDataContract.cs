using System;
using System.Runtime.Serialization;

namespace Randao.DataContracts
{
	[Serializable, DataContract(Name = "article_content")]
	public class ArticleContentDataContract
	{
		[DataMember(Name = "article_id")]
		public long ArticleID { get; set; }

		[DataMember(Name = "category_id")]
		public int CategoryId { get; set; }

		[DataMember(Name = "category_id_list")]
		public string CategoryIdList { get; set; }

		[DataMember(Name = "title")]
		public string Title { get; set; }

		[DataMember(Name = "author")]
		public string Author { get; set; }

		//[DataMember(Name = "description")]
		public string Description { get; set; }

		private string articleContent;

		[DataMember(Name = "article_content")]
		public string ArticleContent
		{
			get
			{
				if (articleContent == null)
				{
					return string.Empty;
				}
				return articleContent.Replace("<img src=\"/BookFiles/", "<img src=\"http://m.randao.com/BookFiles/");
			}
			set
			{
				articleContent = value;
			}
		}

		[DataMember(Name = "post_time")]
		public DateTime PostTime { get; set; }

		[DataMember(Name = "update_time")]
		public DateTime UpdateTime { get; set; }

		[DataMember(Name = "week_hit_count")]
		public long WeekHitCount { get; set; }

		[DataMember(Name = "month_hit_count")]
		public long MonthHitCount { get; set; }

		[DataMember(Name = "hit_count")]
		public long HitCount { get; set; }

		//[DataMember(Name = "hit_time")]
		public DateTime HitTime { get; set; }

		[DataMember(Name = "flower_count")]
		public long FlowerCount { get; set; }

		[DataMember(Name = "egg_count")]
		public long EggCount { get; set; }

		[DataMember(Name = "comment_count")]
		public long CommentCount { get; set; }

		[DataMember(Name = "collect_count")]
		public long CollectCount { get; set; }

		//[DataMember(Name = "is_collect")]
		public bool IsCollect { get; set; }

		//[DataMember(Name = "warn_count")]
		public long WarnCount { get; set; }

		[DataMember(Name = "user_key_id")]
		public long UserKeyId { get; set; }

		//[DataMember(Name = "is_verify")]
		public bool Isverify { get; set; }

		[DataMember(Name = "is_high_priority")]
		public bool IsHighPriority { get; set; }
		
		[DataMember(Name = "face_img")]
		public string FaceImg { get; set; }
	}
}
