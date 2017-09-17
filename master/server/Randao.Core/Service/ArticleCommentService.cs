using Randao.Core.Repositories;
using Randao.DataContracts;
using System;
using System.Linq;

namespace Randao.Core.Service
{
	/// <summary>
	/// 帖子评论业务类
	/// </summary>
	public class ArticleCommentService
	{
		private MemberService memberService;
		private ArticleContentService articleContentService;
		private ArticleCategoryService articleCategoryService;
		public ArticleCommentService(ArticleCategoryService _articleCategoryService, ArticleContentService _articleContentService, MemberService _memberService)
		{
			this.memberService = _memberService;
			this.articleContentService = _articleContentService;
			this.articleCategoryService = _articleCategoryService;
		}

		public ReturnResult<PageList<ArticleCommentDataContract>> GetArticleCommentList(long articleID, int page = 1, int pageSize = 10)
		{
			if (page < 1)
			{
				page = 1;
			}

			var list = ArticleCommentRepository.Instance.GetArticleCommentList(articleID, page, pageSize);

			if (list.DataList.Any())
			{
				var userList = memberService.GetMember(list.DataList.Select(t => t.UserKeyId)).ToDictionary(c => c.UserKeyId, c => c);
				foreach (var item in list.DataList)
				{
					if (userList.ContainsKey(item.UserKeyId))
					{
						item.FaceImg = userList[item.UserKeyId].FaceImg;
						item.NickName = userList[item.UserKeyId].NickName ?? userList[item.UserKeyId].UserName;
					}
				}
			}

			return new ReturnResult<PageList<ArticleCommentDataContract>>(list);
		}


		public ReturnResult<long> InsertArticleComment(ArticleCommentDataContract model, int articleCategoryId)
		{
			if (null == model)
			{
				return new ReturnResult<long>(101, 0, "参数异常,未找到数据");
			}
			if (string.IsNullOrWhiteSpace(model.CommentContent))
			{
				return new ReturnResult<long>(102, 0, "参数异常,内容不允许为空");
			}
			int rootCategoryId = articleCategoryService.GetRootCategoryId(articleCategoryId);

			if (rootCategoryId == -1)
			{
				return new ReturnResult<long>(103, 0, "参数异常,文章分类错误");
			}

			model.PostTime = DateTime.Now;
			model.UpdateTime = DateTime.Now;
			model.TableName = "SA_ArticleContent" + rootCategoryId;

			if (ArticleCommentRepository.Instance.InsertArticleComment(model))
			{
				System.Threading.Tasks.Task.Factory.StartNew(() => articleContentService.UpdateArticleCount(model.ArticleID, articleCategoryId, model.UserKeyId, ArticleCountEnum.CommentCount));
				return new ReturnResult<long>(model.CommentID);
			}
			else
			{
				return new ReturnResult<long>(104, 0, "评论发表失败");
			}
		}
	}
}
