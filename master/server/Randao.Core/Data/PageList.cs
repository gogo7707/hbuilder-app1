using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Randao.Core
{
	[Serializable, DataContract(Name = " page_list")]
	public class PageList<T>
	{
		public PageList()
		{
			this.DataList = new List<T>();
		}

		/// <summary>
		/// 记录总条数
		/// </summary>
		[DataMember(Name = "total_count")]
		public int TotalCount { get; set; }

		/// <summary>
		/// 当前页码
		/// </summary>
		[DataMember(Name = "page")]
		public int Page { get; set; }

		/// <summary>
		/// 每页数量
		/// </summary>
		[DataMember(Name = "page_size")]
		public int PageSize { get; set; }

		/// <summary>
		/// 分页数据
		/// </summary>
		[DataMember(Name = "data_list")]
		public List<T> DataList { get; set; }

		[DataMember(Name = "page_count")]
		public int PageCount
		{
			get
			{
				return (int)Math.Ceiling(TotalCount * 1.0 / this.PageSize);
			}
		}
	}
}
