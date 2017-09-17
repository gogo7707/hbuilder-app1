using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randao.DataContracts
{
	public class ReviewRecordDataContract
	{
		public long ArticleID { get; set; }

		public int CategoryId { get; set; }

		public long UserKeyId { get; set; }

		public ArticleCountEnum ReviewType { get; set; }

		public string Title { get; set; }

		public long AuthorUserKeyId { get; set; }

		public DateTime PostTime { get; set; }
	}
}
