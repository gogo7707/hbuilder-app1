using Randao.Core.Data;
using Randao.DataContracts;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Randao.Core.Repositories
{
	internal class ArticleAttentionRepository : Singleton<ArticleAttentionRepository>
	{
		private ArticleAttentionRepository() { }

		public bool InsertArticleAttention(ArticleAttentionDataContract articleAttention)
		{
			string sql = "INSERT INTO SA_ArticleAttention(UserKeyId,AttentionUserKeyId,AttentionTime) VALUES(@userKeyId,@attentionUserKeyId,@attentionTime);SELECT @@IDENTITY;";

			var _parm = new SqlParameter[] { 
				new SqlParameter("@userKeyId", articleAttention.UserKeyId),
				new SqlParameter("@attentionUserKeyId", articleAttention.AttentionUserKeyId),
				new SqlParameter("@attentionTime",articleAttention.AttentionTime)
			};

			articleAttention.AttentionId = Convert.ToInt64(SqlHelper.ExecuteScalar(SqlHelper.GetConnSting(), CommandType.Text, sql, _parm));

			return articleAttention.AttentionId > 0;
		}

		public bool IsExists(long userKeyId, long attentionUserKeyId)
		{
			string sql = "SELECT 1 FROM SA_ArticleAttention WHERE UserKeyId=@userKeyId AND AttentionUserKeyId=@attentionUserKeyId";

			var _parm = new SqlParameter[] { 
				new SqlParameter("@userKeyId",userKeyId),
				new SqlParameter("@attentionUserKeyId",attentionUserKeyId)
			};

			return Convert.ToInt64(SqlHelper.ExecuteScalar(SqlHelper.GetConnSting(), CommandType.Text, sql, _parm)) == 1;
		}


		public PageList<ArticleAttentionDataContract> GetArticleAttention(long userKeyId, int page, int pageSize)
		{
			var _list = new PageList<ArticleAttentionDataContract>() { Page = page, PageSize = pageSize };

			string sql = @"SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY AttentionId DESC) AS SortId,AttentionId,UserKeyId,AttentionUserKeyId,AttentionTime FROM SA_ArticleAttention WHERE UserKeyId = @userKeyId) AS temp WHERE sortId BETWEEN @begin AND @end";

			var _parm = new SqlParameter[] { 
				new SqlParameter("@userKeyId", userKeyId),
				new SqlParameter("@begin", (page - 1) * pageSize + 1),
				new SqlParameter("@end", page * pageSize)
			};

			_list.TotalCount = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.GetConnSting(), CommandType.Text, "SELECT COUNT(*) FROM SA_ArticleAttention WHERE userKeyId = @userKeyId", _parm));

			if (_list.TotalCount > 0)
			{
				using (var dr = SqlHelper.ExecuteReader(SqlHelper.GetConnSting(), CommandType.Text, sql, _parm))
				{
					_list.DataList = EntityHelper.GetList<ArticleAttentionDataContract>(dr);
				}
			}

			return _list;
		}
	}
}
