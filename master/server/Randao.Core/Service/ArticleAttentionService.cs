using Randao.Core.Repositories;
using Randao.DataContracts;
using System.Linq;

namespace Randao.Core.Service
{
	public class ArticleAttentionService
	{
		private MemberService memberService;
		public ArticleAttentionService(MemberService _memberService)
		{
			this.memberService = _memberService;
		}

		public ReturnResult<bool> AddArticleAttention(ArticleAttentionDataContract articleAttention)
		{
			if (articleAttention.AttentionUserKeyId == articleAttention.UserKeyId)
			{
				return new ReturnResult<bool>(101, false, "不允许关注自己");
			}
			if (ArticleAttentionRepository.Instance.IsExists(articleAttention.UserKeyId, articleAttention.AttentionUserKeyId))
			{
				return new ReturnResult<bool>(true);
			}

			return new ReturnResult<bool>(ArticleAttentionRepository.Instance.InsertArticleAttention(articleAttention));
		}

		public ReturnResult<PageList<ArticleAttentionDataContract>> GetArticleAttention(long userKeyId, int page, int pageSize)
		{
			if (page < 1)
			{
				page = 1;
			}


			var list = ArticleAttentionRepository.Instance.GetArticleAttention(userKeyId, page, pageSize);

			if (list.DataList.Any())
			{
				var userList = memberService.GetMember(list.DataList.Select(t => t.AttentionUserKeyId)).ToDictionary(c => c.UserKeyId, c => c);
				list.DataList.ForEach(t =>
				{
					if (userList.ContainsKey(t.AttentionUserKeyId))
					{
						t.AttentionUserFaceImg = userList[t.AttentionUserKeyId].FaceImg;
						t.AttentionUserKeyName = userList[t.AttentionUserKeyId].NickName ?? userList[t.AttentionUserKeyId].UserName;
					}
				});
			}

			return new ReturnResult<PageList<ArticleAttentionDataContract>>(list);
		}
	}
}
