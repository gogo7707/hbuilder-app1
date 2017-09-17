using System;
using System.Runtime.Serialization;

namespace Randao.DataContracts
{
	/// <summary>
	/// 帖子分类表
	/// </summary>
	[Serializable, DataContract(Name = " article_category")]
	public class ArticleCategoryDataContract
	{
		[DataMember(Name = "category_id")]
		public Int32 CategoryId { get; set; }
		public Int32 ParentId { get; set; }
		public Int32 Sequence { get; set; }
		public System.DateTime PostTime { get; set; }
		public System.DateTime UpdateTime { get; set; }
		public Boolean IsWapShow { get; set; }
		public String parentIdList { get; set; }
		public String CategoryName { get; set; }

		[DataMember(Name = "category_name")]
		public String CategoryOtherName { get; set; }
		public String description { get; set; }
	}
}
