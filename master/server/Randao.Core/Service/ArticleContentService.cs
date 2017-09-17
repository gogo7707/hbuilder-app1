using Randao.Core.Repositories;
using Randao.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Randao.Core.Service
{
	/// <summary>
	/// 帖子业务类
	/// </summary>
	public class ArticleContentService
	{
		private MemberService memberService;
		private ArticleCategoryService articleCategoryService;
		public ArticleContentService(ArticleCategoryService _articleCategoryService, MemberService _memberService)
		{
			this.memberService = _memberService;
			this.articleCategoryService = _articleCategoryService;
		}

		public ReturnResult<bool> UpdateArticleCount(long articleId, int categoryId, long userKeyId, ArticleCountEnum articleCountEnum)
		{
			int rootCategoryId = articleCategoryService.GetRootCategoryId(categoryId);

			if (rootCategoryId == -1)
			{
				return new ReturnResult<bool>(101, false, "分类参数错误");
			}

			var isNeedVerification = articleCountEnum == ArticleCountEnum.EggCount || articleCountEnum == ArticleCountEnum.FlowerCount || articleCountEnum == ArticleCountEnum.WarnCount;

			if (isNeedVerification && ArticleContentRepository.Instance.IsReview(articleId, categoryId, userKeyId, (int)articleCountEnum))
			{
				return new ReturnResult<bool>(102, false, "已经有过相同操作,不允许重复");
			}

			var ret = ArticleContentRepository.Instance.UpdateArticleCount(articleId, rootCategoryId, articleCountEnum);

			if (isNeedVerification && ret)
			{
				var article = ArticleContentRepository.Instance.GetArticle(rootCategoryId, articleId);
				if (null != article)
				{
					System.Threading.Tasks.Task.Factory.StartNew(() => ArticleContentRepository.Instance.InsertReViewRecord(new ReviewRecordDataContract()
					{
						ArticleID = articleId,
						CategoryId = categoryId,
						AuthorUserKeyId = article.UserKeyId,
						ReviewType = articleCountEnum,
						Title = article.Title,
						UserKeyId = userKeyId,
						PostTime = DateTime.Now
					}));
				}
			}

			return new ReturnResult<bool>(ret);
		}

		public ReturnResult<PageList<ArticleContentDataContract>> GetArticleContentList(int categoryId, long userKeyId = 0, bool? isverify = true, int page = 1, int pageSize = 10)
		{
			int rootCategoryId = articleCategoryService.GetRootCategoryId(categoryId);

			if (rootCategoryId == -1)
			{
				return new ReturnResult<PageList<ArticleContentDataContract>>(101, null, "分类ID错误");
			}

			if (rootCategoryId == categoryId)
			{
				categoryId = 0;
			}

			var list = ArticleContentRepository.Instance.GetArticleContentList(rootCategoryId, categoryId, userKeyId, isverify, page, pageSize);

			if (list.DataList.Any())
			{
				var userList = memberService.GetMember(list.DataList.Select(t => t.UserKeyId)).ToDictionary(c => c.UserKeyId, c => c);
				list.DataList.ForEach(t =>
				{
					if (userList.ContainsKey(t.UserKeyId))
					{
						t.FaceImg = userList[t.UserKeyId].FaceImg;
					}
				});
			}

			return new ReturnResult<PageList<ArticleContentDataContract>>(list);
		}

		public ReturnResult<PageList<ArticleContentDataContract>> GetNoVerifyArticle(int categoryId, long userKeyId, int page = 1, int pageSize = 10)
		{
			int rootCategoryId = articleCategoryService.GetRootCategoryId(categoryId);

			if (rootCategoryId == -1)
			{
				return new ReturnResult<PageList<ArticleContentDataContract>>(101, null, "分类ID错误");
			}

			if (rootCategoryId == categoryId)
			{
				categoryId = 0;
			}

			var list = ArticleContentRepository.Instance.GetNoVerifyArticle(rootCategoryId, categoryId, userKeyId, page, pageSize);

			if (list.DataList.Any())
			{
				var userList = memberService.GetMember(list.DataList.Select(t => t.UserKeyId)).ToDictionary(c => c.UserKeyId, c => c);
				list.DataList.ForEach(t =>
				{
					if (userList.ContainsKey(t.UserKeyId))
					{
						t.FaceImg = userList[t.UserKeyId].FaceImg;
					}
				});
			}

			return new ReturnResult<PageList<ArticleContentDataContract>>(list);
		}

		public ReturnResult<ArticleContentDataContract> GetArticle(int categoryId, long articleID)
		{
			int rootCategoryId = articleCategoryService.GetRootCategoryId(categoryId);

			if (rootCategoryId == -1)
			{
				return new ReturnResult<ArticleContentDataContract>(101, null, "分类ID错误");
			}

			var article = ArticleContentRepository.Instance.GetArticle(rootCategoryId, articleID);

			if (article != null)
			{
				var userList = memberService.GetMember(new List<long>() { article.UserKeyId });
				article.FaceImg = userList.Any() ? userList.FirstOrDefault().FaceImg : null;
			}

			return new ReturnResult<ArticleContentDataContract>(article);
		}

        public ReturnResult<long> PublishArticle(ArticleContentDataContract article)
        {
            if (null == article)
            {
                return new ReturnResult<long>(101, 0, "数据错误");
            }

            if (string.IsNullOrWhiteSpace(article.Title))
            {
                return new ReturnResult<long>(104, 0, "帖子标题不允许为空");
            }

            if (string.IsNullOrWhiteSpace(article.ArticleContent))
            {
                return new ReturnResult<long>(102, 0, "帖子内容不允许为空");
            }

            var parentCategoryList = articleCategoryService.GetParentCategoryList(article.CategoryId);

            if (!parentCategoryList.Any())
            {
                return new ReturnResult<long>(103, 0, "文章分类ID错误,未找到指定的ID");
            }

            int rootCategoryId = parentCategoryList.First();

            rootCategoryId = rootCategoryId == 0 ? article.CategoryId : rootCategoryId;

            parentCategoryList.Add(article.CategoryId);

            article.CategoryIdList = string.Join("|", parentCategoryList);
            article.PostTime = DateTime.Now;
            article.UpdateTime = DateTime.Now;
            article.HitTime = DateTime.Now;
            article.Description = article.ArticleContent.RemoveHtmlTag().CutString(480);

            if (article.CategoryId == 390)
            {
                article.Title = "[视频]";
            }
            else if (article.CategoryId == 388)
            {
                article.Title = "[图片]";
            }
            else
            {
                article.Title = string.IsNullOrEmpty(article.Description) ? article.Author : article.Description.CutString(20);
            }

			article.Title = article.Title ?? "";


            if (ArticleContentRepository.Instance.Insert(rootCategoryId, article))
            {
                return new ReturnResult<long>(article.ArticleID);
            }
            else
            {
                return new ReturnResult<long>(102, 0, "发布失败.");
            }
        }
	}
}
