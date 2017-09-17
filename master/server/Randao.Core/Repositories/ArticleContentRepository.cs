using Randao.Core.Data;
using Randao.DataContracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Randao.Core.Repositories
{
	public class ArticleContentRepository : Singleton<ArticleContentRepository>
	{
		private ArticleContentRepository() { }

		public PageList<ArticleContentDataContract> GetArticleContentList(int rootCategoryId, int categoryId, long userKeyId = 0, bool? isverify = true, int page = 1, int pageSize = 10)
		{
			var _parm = new List<SqlParameter>();

			string condition = string.Empty;

			if (categoryId > 0)
			{
				condition += " AND CategoryId=@categoryId ";
				_parm.Add(new SqlParameter("@categoryId", categoryId));
			}
			if (userKeyId > 0)
			{
				condition += " AND UserKeyId=@userKeyId ";
				_parm.Add(new SqlParameter("@userKeyId", userKeyId));
			}
			if (isverify.HasValue && isverify.Value)
			{
				condition += " AND (IsCollect=1 or Isverify = 1)";
			}
			if (isverify.HasValue && !isverify.Value)
			{
				condition += " AND (IsCollect=0 and Isverify = 0)";
			}

			_parm.Add(new SqlParameter("@begin", (page - 1) * pageSize + 1));
			_parm.Add(new SqlParameter("@end", page * pageSize));

            string sql = @"SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY IsHighPriority,UpdateTime DESC) AS SortId,ArticleID,CategoryId,CategoryIdList,Title,Author,Description,
						   ArticleContent,PostTime,UpdateTime,WeekHitCount,MonthHitCount,HitCount,HitTime,FlowerCount,EggCount,CommentCount,CollectCount,IsCollect,
						   Isverify,WarnCount,UserKeyId,IsHighPriority FROM SA_ArticleContent{0} WHERE 1=1 {1}) AS temp WHERE sortId BETWEEN @begin AND @end";

			sql = string.Format(sql, rootCategoryId, condition);

			var _list = new PageList<ArticleContentDataContract>() { Page = page, PageSize = pageSize };

			_list.TotalCount = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.GetConnSting(), CommandType.Text, string.Format("SELECT COUNT(*) FROM SA_ArticleContent{0} WHERE 1=1 {1}", rootCategoryId, condition), _parm.ToArray()));

			if (_list.TotalCount > 0)
			{
				using (var dr = SqlHelper.ExecuteReader(SqlHelper.GetConnSting(), CommandType.Text, sql, _parm.ToArray()))
				{
					_list.DataList = EntityHelper.GetList<ArticleContentDataContract>(dr);
				}
			}

			return _list;
		}

		public PageList<ArticleContentDataContract> GetNoVerifyArticle(int rootCategoryId, int categoryId, long userKeyId, int page = 1, int pageSize = 10)
		{
			var _parm = new List<SqlParameter>() { 
				new SqlParameter("@userKeyId", userKeyId),
				new SqlParameter("@begin", (page - 1) * pageSize + 1),
				new SqlParameter("@end", page * pageSize)
			};

			string condition = string.Empty;

            string sql = @"SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY SA_Article.IsHighPriority,SA_Article.UpdateTime DESC) AS SortId,SA_Article.* FROM SA_ArticleContent{0} as SA_Article
						   LEFT JOIN review_record ON SA_Article.ArticleID = review_record.ArticleID AND SA_Article.CategoryId = review_record.CategoryId
                           WHERE {1} SA_Article.IsCollect = 0 AND Isverify = 0 AND review_record.UserKeyId IS NULL OR review_record.UserKeyId != @userKeyId) AS temp WHERE sortId BETWEEN @begin AND @end";

			if (categoryId > 0)
			{
				condition += " SA_Article.CategoryId=@categoryId AND ";
				_parm.Add(new SqlParameter("@categoryId", categoryId));
			}

			sql = string.Format(sql, rootCategoryId, condition);

			var _list = new PageList<ArticleContentDataContract>() { Page = page, PageSize = pageSize };

			string sqlPage = string.Format(@"SELECT COUNT(*) FROM SA_ArticleContent{0} as SA_Article
										     LEFT JOIN review_record ON SA_Article.ArticleID = review_record.ArticleID AND SA_Article.CategoryId = review_record.CategoryId
									         WHERE {1} SA_Article.IsCollect = 0 AND SA_Article.Isverify = 0 AND review_record.UserKeyId IS NULL OR review_record.UserKeyId != 6", rootCategoryId, condition);

			_list.TotalCount = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.GetConnSting(), CommandType.Text, sqlPage, _parm.ToArray()));

			if (_list.TotalCount > 0)
			{
				using (var dr = SqlHelper.ExecuteReader(SqlHelper.GetConnSting(), CommandType.Text, sql, _parm.ToArray()))
				{
					_list.DataList = EntityHelper.GetList<ArticleContentDataContract>(dr);
				}
			}

			return _list;
		}

		public ArticleContentDataContract GetArticle(int rootCategoryId, long articleID)
		{
			string sql = @"SELECT ArticleID,CategoryId,CategoryIdList,Title,Author,Description,ArticleContent,PostTime,UpdateTime,WeekHitCount,MonthHitCount,HitCount,HitTime,FlowerCount,
						   EggCount,CommentCount,CollectCount,IsCollect,Isverify,WarnCount,UserKeyId,IsHighPriority FROM SA_ArticleContent{0} WHERE ArticleID=@articleID";

			sql = string.Format(sql, rootCategoryId);

			ArticleContentDataContract article;

			using (var dr = SqlHelper.ExecuteReader(SqlHelper.GetConnSting(), CommandType.Text, sql, new SqlParameter("@articleID", articleID)))
			{
				article = EntityHelper.GetEntity<ArticleContentDataContract>(dr);
			}

			return article;
		}

		public bool Insert(int rootCategoryId, ArticleContentDataContract article)
		{
			string sql = @"INSERT INTO SA_ArticleContent{0}(CategoryId,CategoryIdList,Title,Author,Description,ArticleContent,PostTime,UpdateTime,
						   WeekHitCount,MonthHitCount,HitCount,HitTime,FlowerCount,EggCount,CommentCount,CollectCount,IsCollect,WarnCount,UserKeyId,Isverify,IsHighPriority)
						   VALUES(@CategoryId,@CategoryIdList,@Title,@Author,@Description,@ArticleContent,@PostTime,@UpdateTime,
						   @WeekHitCount,@MonthHitCount,@HitCount,@HitTime,@FlowerCount,@EggCount,@CommentCount,@CollectCount,@IsCollect,@WarnCount,@UserKeyId,@Isverify,@IsHighPriority);
						   SELECT @@IDENTITY;";

			sql = string.Format(sql, rootCategoryId);

			var _parm = new SqlParameter[] { 
				new SqlParameter("@CategoryId", article.CategoryId),
				new SqlParameter("@CategoryIdList", article.CategoryIdList),
				new SqlParameter("@Title", article.Title??string.Empty),
				new SqlParameter("@Author", article.Author??string.Empty),
				new SqlParameter("@Description", article.Description??string.Empty),
				new SqlParameter("@ArticleContent", article.ArticleContent??string.Empty),
				new SqlParameter("@PostTime", article.PostTime),
				new SqlParameter("@UpdateTime", article.UpdateTime),
				new SqlParameter("@WeekHitCount", article.WeekHitCount),
				new SqlParameter("@MonthHitCount", article.MonthHitCount),
				new SqlParameter("@HitCount", article.HitCount),
				new SqlParameter("@HitTime", article.HitTime),
				new SqlParameter("@FlowerCount", article.FlowerCount),
				new SqlParameter("@EggCount", article.EggCount),
				new SqlParameter("@CommentCount", article.CommentCount),
				new SqlParameter("@CollectCount", article.CollectCount),
				new SqlParameter("@IsCollect", article.IsCollect),
				new SqlParameter("@WarnCount", article.WarnCount),
				new SqlParameter("@UserKeyId", article.UserKeyId),
				new SqlParameter("@Isverify", article.Isverify),
				new SqlParameter("@IsHighPriority", article.IsHighPriority)
			};

			article.ArticleID = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.GetConnSting(), CommandType.Text, sql, _parm));

			return article.ArticleID > 0;
		}

		public bool UpdateArticleCount(long articleID, int rootCategoryId, ArticleCountEnum articleCountEnum)
		{
			string sql = "UPDATE SA_ArticleContent" + rootCategoryId + " SET {0} WHERE ArticleID = @articleID";

			switch (articleCountEnum)
			{
				case ArticleCountEnum.CollectCount:
					sql = string.Format(sql, "CollectCount = CollectCount + 1");
					break;
				case ArticleCountEnum.CommentCount:
					sql = string.Format(sql, "CommentCount = CommentCount + 1");
					break;
				case ArticleCountEnum.EggCount:
					sql = string.Format(sql, "EggCount = EggCount + 1");
					break;
				case ArticleCountEnum.FlowerCount:
					sql = string.Format(sql, "FlowerCount = FlowerCount + 1");
					break;
				case ArticleCountEnum.HitCount:
					sql = string.Format(sql, "WeekHitCount = WeekHitCount + 1,MonthHitCount= MonthHitCount+1, HitCount = HitCount + 1,HitTime = getdate()");
					break;
				default:
					return false;
			}

			return SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, sql, new SqlParameter("@articleID", articleID)) > 0;
		}

		public bool InsertReViewRecord(ReviewRecordDataContract reviewRecord)
		{
			string sql = @"INSERT INTO review_record(ArticleID,CategoryId,UserKeyId,ReviewType,Title,AuthorUserKeyId,PostTime)
						   VALUES(@ArticleID,@CategoryId,@UserKeyId,@ReviewType,@Title,@AuthorUserKeyId,@PostTime)";

			var _parm = new SqlParameter[] { 
				new SqlParameter("@ArticleID", reviewRecord.ArticleID),
				new SqlParameter("@CategoryId", reviewRecord.CategoryId),
				new SqlParameter("@UserKeyId", reviewRecord.UserKeyId),
				new SqlParameter("@ReviewType",(int) reviewRecord.ReviewType),
				new SqlParameter("@Title", reviewRecord.Title),
				new SqlParameter("@AuthorUserKeyId", reviewRecord.AuthorUserKeyId),
				new SqlParameter("@PostTime", reviewRecord.PostTime)
			};

			return SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, sql, _parm) > 0;
		}

		public bool IsReview(long articleID, int categoryId, long userKeyId, int ReviewType)
		{
			string sql = "SELECT TOP 1 1 FROM review_record WHERE ArticleID=@ArticleID AND CategoryId=@CategoryId AND UserKeyId=@UserKeyId AND ReviewType=@ReviewType";

			var _parm = new SqlParameter[] { 
				new SqlParameter("@ArticleID",articleID),
				new SqlParameter("@CategoryId",categoryId),
				new SqlParameter("@UserKeyId",userKeyId),
				new SqlParameter("@ReviewType",ReviewType)
			};

			return Convert.ToInt64(SqlHelper.ExecuteScalar(SqlHelper.GetConnSting(), CommandType.Text, sql, _parm)) == 1;
		}
	}
}
