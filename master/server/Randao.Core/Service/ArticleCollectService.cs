using Randao.Core.Repositories;
using Randao.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randao.Core.Service
{
    /// <summary>
    /// 收藏业务类
    /// </summary>
    public class ArticleCollectService
    {
        private MemberService memberService;
        private ArticleCategoryService articleCategoryService;
        private ArticleContentService articleContentService;
        public ArticleCollectService(ArticleCategoryService _articleCategoryService, MemberService _memberService, ArticleContentService _articleContentService)
        {
            this.memberService = _memberService;
            this.articleCategoryService = _articleCategoryService;
            this.articleContentService = _articleContentService;
        }

        /// <summary>
        /// 获取我收藏的帖子
        /// </summary>
        public PageList<ArticleCollectDataContract> GetArticleCollect(long userKeyId, int page, int pageSize)
        {
            var list = ArticleCollectRepository.Instance.GetArticleCollect(userKeyId, page, pageSize);

            if (list.DataList.Any())
            {
                var userList = memberService.GetMember(list.DataList.Select(t => t.AuthorUserKeyId)).ToDictionary(c => c.UserKeyId, c => c);
                foreach (var item in list.DataList)
                {
                    if (userList.ContainsKey(item.AuthorUserKeyId))
                    {
                        item.FaceImg = userList[item.AuthorUserKeyId].FaceImg;
                        item.Author = userList[item.AuthorUserKeyId].NickName ?? userList[item.AuthorUserKeyId].UserName;
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 收藏帖子
        /// </summary>
        public ReturnResult<bool> AddArticleCollect(ArticleCollectDataContract articleCollect, int categoryId)
        {
            int rootCategoryId = articleCategoryService.GetRootCategoryId(categoryId);

            if (rootCategoryId == -1)
            {
                return new ReturnResult<bool>(101, false, "分类参数错误");
            }
            articleCollect.TableName = "SA_ArticleContent" + rootCategoryId;
            if (ArticleCollectRepository.Instance.IsExists(articleCollect.ArticleID, articleCollect.TableName))
            {
                return new ReturnResult<bool>(true);
            }
            var article = ArticleContentRepository.Instance.GetArticle(rootCategoryId, articleCollect.ArticleID);
            if (null == article)
            {
                return new ReturnResult<bool>(102, false, "未找到指定的帖子");
            }
            articleCollect.Title = article.Title;
            articleCollect.Author = article.Author;
            articleCollect.AuthorUserKeyId = article.UserKeyId;
            articleCollect.CollectTime = DateTime.Now;

            if (ArticleCollectRepository.Instance.InsertArticleCollect(articleCollect))
            {
                System.Threading.Tasks.Task.Factory.StartNew(() => articleContentService.UpdateArticleCount(article.ArticleID, article.CategoryId, articleCollect.UserKeyId, ArticleCountEnum.CollectCount));
                return new ReturnResult<bool>(true);
            }
            return new ReturnResult<bool>(103, false, "收藏失败");
        }


        /// <summary>
        /// 删除收藏的帖子
        /// </summary>
        public ReturnResult<bool> DeleteArticleCollect(long userKeyId, long collectId)
        {
            return new ReturnResult<bool>(ArticleCollectRepository.Instance.DeleteArticleCollect(userKeyId, collectId));
        }

        /// <summary>
        /// 获取收藏的帖子数量
        /// </summary>
        /// <param name="userKeyId"></param>
        /// <returns></returns>
        public ReturnResult<int> GetArticleCollectCount(long userKeyId)
        {
            return new ReturnResult<int>(ArticleCollectRepository.Instance.GetArticleCollectCount(userKeyId));
        }
    }
}
