using System.Data;
using System.Data.SqlClient;
using Randao.DataContracts;
using Randao.Core.Data;
using System.Collections.Generic;

namespace Randao.Core.Repositories
{
	internal class ArticleCategoryRepository : Singleton<ArticleCategoryRepository>
	{
		private ArticleCategoryRepository() { }

		public ArticleCategoryDataContract GetArticleCategory(int categoryId)
		{
			string sql = "SELECT CategoryId,ParentId,parentIdList,CategoryName,CategoryOtherName,Sequence,PostTime,UpdateTime,description,IsWapShow FROM SA_ArticleCategory WHERE CategoryId=@categoryId";

			ArticleCategoryDataContract articleCategory = null;

			using (var dr = SqlHelper.ExecuteReader(SqlHelper.GetConnSting(), CommandType.Text, sql, new SqlParameter("categoryId", categoryId)))
			{
				articleCategory = EntityHelper.GetEntity<ArticleCategoryDataContract>(dr);
			}

			return articleCategory;
		}

		public List<ArticleCategoryDataContract> GetWapArticleCategoryList()
		{
			string sql = @"SELECT CategoryId,ParentId,parentIdList,CategoryName,CategoryOtherName,Sequence,PostTime,UpdateTime,
						   Description,IsWapShow FROM SA_ArticleCategory WHERE IsWapShow=1 ORDER BY Sequence ASC";

			var _list = new List<ArticleCategoryDataContract>();

			using (var dr = SqlHelper.ExecuteReader(SqlHelper.GetConnSting(), CommandType.Text, sql))
			{
				_list = EntityHelper.GetList<ArticleCategoryDataContract>(dr);
			}

			return _list;
		}
	}
}
