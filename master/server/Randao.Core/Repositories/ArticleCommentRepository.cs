using Randao.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Randao.Core.Data;

namespace Randao.Core.Repositories
{
	internal class ArticleCommentRepository : Singleton<ArticleCommentRepository>
    {
		private ArticleCommentRepository() { }

        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <returns></returns>
		public PageList<ArticleCommentDataContract> GetArticleCommentList(long articleID, int page = 1, int pageSize = 10)
		{
			string sql = @"SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY CommentID ASC) AS SortId,CommentID,ArticleID,CommentContent,PostTime,UpdateTime,FlowerCount,EggCount,
                           WarnCount,TableName,UserKeyId FROM SA_ArticleComment WHERE ArticleID = @articleID) AS temp WHERE sortId BETWEEN @begin AND @end";

			var _list = new PageList<ArticleCommentDataContract>() { Page = page, PageSize = pageSize };

			var _parm = new SqlParameter[] { 
				new SqlParameter("@articleID", articleID),
				new SqlParameter("@begin", (page - 1) * pageSize + 1),
				new SqlParameter("@end", page * pageSize)
			};

			_list.TotalCount = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.GetConnSting(), CommandType.Text, "SELECT COUNT(*) FROM SA_ArticleComment WHERE ArticleID = @articleID", _parm));

			if (_list.TotalCount > 0)
			{
				using (var dr = SqlHelper.ExecuteReader(SqlHelper.GetConnSting(), CommandType.Text, sql, _parm))
				{
					_list.DataList = EntityHelper.GetList<ArticleCommentDataContract>(dr);
				}
			}

			return _list;
		}

        /// <summary>
        /// 新增评论
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
		public bool InsertArticleComment(ArticleCommentDataContract model)
		{
			string sql = @"INSERT INTO SA_ArticleComment(ArticleID,CommentContent,PostTime,UpdateTime,FlowerCount,EggCount,WarnCount,TableName,UserKeyId)
                           VALUES(@ArticleID,@CommentContent,@PostTime,@UpdateTime,@FlowerCount,@EggCount,@WarnCount,@TableName,@UserKeyId);SELECT @@IDENTITY;";

			var _parm = new SqlParameter[] { 
				new SqlParameter("@ArticleID", model.ArticleID),
				new SqlParameter("@CommentContent", model.CommentContent),
				new SqlParameter("@PostTime", model.PostTime),
				new SqlParameter("@UpdateTime", model.UpdateTime),
				new SqlParameter("@FlowerCount", model.FlowerCount),
				new SqlParameter("@EggCount", model.EggCount),
				new SqlParameter("@WarnCount", model.WarnCount),
				new SqlParameter("@TableName", model.TableName),
				new SqlParameter("@UserKeyId", model.UserKeyId)
			};

			model.CommentID = Convert.ToInt64(SqlHelper.ExecuteScalar(SqlHelper.GetConnSting(), CommandType.Text, sql, _parm));

			return model.CommentID > 0;
		}

		public bool DeleteArticleComment(long id)
		{
			return false;
		}
    }
}
