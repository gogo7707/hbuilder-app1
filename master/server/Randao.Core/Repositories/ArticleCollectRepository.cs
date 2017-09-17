using Randao.Core.Data;
using Randao.DataContracts;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Randao.Core.Repositories
{
	internal class ArticleCollectRepository : Singleton<ArticleCollectRepository>
	{
		private ArticleCollectRepository() { }

		public PageList<ArticleCollectDataContract> GetArticleCollect(long userKeyId, int page, int pageSize)
		{
			string sql = @"SELECT * FROM (
								SELECT ROW_NUMBER() OVER(ORDER BY CollectId DESC) AS SortId,CollectId,ArticleID,TableName,UserKeyId,Title,Author,AuthorUserKeyId,CollectTime FROM SA_ArticleCollect WHERE UserKeyId = @userKeyId
						   ) AS temp WHERE sortId BETWEEN @begin AND @end";

			var _list = new PageList<ArticleCollectDataContract>() { Page = page, PageSize = pageSize };

			var _parm = new SqlParameter[] { 
				new SqlParameter("@userKeyId", userKeyId),
				new SqlParameter("@begin", (page - 1) * pageSize + 1),
				new SqlParameter("@end", page * pageSize)
			};

			_list.TotalCount = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.GetConnSting(), CommandType.Text, "SELECT COUNT(*) FROM SA_ArticleCollect WHERE UserKeyId = @userKeyId", _parm));

			if (_list.TotalCount > 0)
			{
				using (var dr = SqlHelper.ExecuteReader(SqlHelper.GetConnSting(), CommandType.Text, sql, _parm))
				{
					_list.DataList = EntityHelper.GetList<ArticleCollectDataContract>(dr);
				}
			}

			return _list;
		}

		public bool IsExists(long articleID, string tableName)
		{
			string sql = "SELECT TOP 1 1 FROM SA_ArticleCollect WHERE ArticleID=@articleID AND TableName=@tableName";

			var _parm = new SqlParameter[] { 
				new SqlParameter("@articleID",articleID),
				new SqlParameter("@tableName",tableName)
			};

			return Convert.ToInt64(SqlHelper.ExecuteScalar(SqlHelper.GetConnSting(), CommandType.Text, sql, _parm)) == 1;
		}

        public int GetArticleCollectCount(long userKeyId)
        {
            string sql = @"SELECT COUNT(*) FROM SA_ArticleCollect WHERE UserKeyId = @userKeyId";

            return Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.GetConnSting(), CommandType.Text, sql, new SqlParameter("@userKeyId", userKeyId)));
        }

		public bool InsertArticleCollect(ArticleCollectDataContract articleCollect)
		{
			string sql = @"INSERT INTO SA_ArticleCollect(ArticleID,TableName,UserKeyId,Title,Author,AuthorUserKeyId,CollectTime)
						   VALUES(@ArticleID,@TableName,@UserKeyId,@Title,@Author,@AuthorUserKeyId,@CollectTime);SELECT @@IDENTITY;";

			var _parm = new SqlParameter[] { 
				new SqlParameter("@ArticleID", articleCollect.ArticleID),
				new SqlParameter("@TableName", articleCollect.TableName),
				new SqlParameter("@UserKeyId", articleCollect.UserKeyId),
				new SqlParameter("@Title",articleCollect.Title),
				new SqlParameter("@Author",articleCollect.Author),
				new SqlParameter("@AuthorUserKeyId",articleCollect.AuthorUserKeyId),
				new SqlParameter("@CollectTime", articleCollect.CollectTime)
			};

			articleCollect.CollectId = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.GetConnSting(), CommandType.Text, sql, _parm));

			return articleCollect.CollectId > 0;
		}

		public bool DeleteArticleCollect(long userKeyId, long collectId)
		{
			string sql = "DELETE FROM SA_ArticleCollect WHERE UserKeyId=@userKeyId AND CollectId=@collectId";

			return SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, sql, new SqlParameter("@userKeyId", userKeyId), new SqlParameter("@collectId", collectId)) > 0;
		}
	}
}
