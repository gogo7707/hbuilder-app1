using Randao.Core;
using Randao.Core.Service;
using Randao.DataContracts;
using System;
using System.Web.Mvc;
using Randao.Core.Extensions;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Randao.Web.Controllers
{
    public class ArticleController : CustomerController
    {
        private const int PAGESIZE = 20;
        private ArticleContentService articleContentService;
        private ArticleCommentService articleCommentService;
        private ArticleCollectService articleCollectService;
        private ArticleAttentionService articleAttentionService;
        private ArticleCategoryService articleCategoryService;

        public ArticleController(ArticleContentService _articleContentService, ArticleCommentService _articleCommentService, ArticleAttentionService _articleAttentionService, ArticleCollectService _articleCollectService, ArticleCategoryService _articleCategoryService)
        {
            this.articleContentService = _articleContentService;
            this.articleCommentService = _articleCommentService;
            this.articleAttentionService = _articleAttentionService;
            this.articleCollectService = _articleCollectService;
            this.articleCategoryService = _articleCategoryService;
        }

        #region  测试接口页面
        public ActionResult PublishTest()
        {
            return View();
        }

        public ActionResult PublishCommentTest()
        {
            return View();
        }

        public ActionResult UploadTest()
        {
            return View();
        }
        #endregion

        #region 帖子相关接口
        /// <summary>
        /// 获取帖子列表接口(不包含帖子具体内容)
        /// </summary>
        public JsonResult Get(int categoryId, int page = 1, int pageSize = PAGESIZE)
        {
            return Json(articleContentService.GetArticleContentList(categoryId, 0, true, page, pageSize));
        }

        /// <summary>
        /// 获取帖子详情
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public JsonResult GetDetail(long articleId, int categoryId)
        {
            return Json(articleContentService.GetArticle(categoryId, articleId));
        }

        /// <summary>
        /// 获取未审核的帖子接口
        /// </summary>
        [OauthAuthorize]
        public JsonResult GetNoVerify(OauthToken token, int categoryId, int page = 1, int pageSize = PAGESIZE)
        {
            return Json(articleContentService.GetNoVerifyArticle(categoryId, token.UserKeyId, page, pageSize));
        }

        /// <summary>
        /// 获取我发布的帖子接口
        /// </summary>
        [OauthAuthorize]
        public JsonResult GetMy(OauthToken token, int categoryId, int page = 1, int pageSize = PAGESIZE)
        {
            return Json(articleContentService.GetArticleContentList(categoryId, token.UserKeyId, null, page, pageSize));
        }

        /// <summary>
        /// 发布帖子接口
        /// </summary>
        [OauthAuthorize, HttpPost, ValidateInput(false)]
        public JsonResult Publish(OauthToken token, int categoryId, string title, string content)
        {
            try
            {
                content = Server.UrlDecode(content);

                if (string.IsNullOrWhiteSpace(content))
                {
                    return Json(new ReturnResult<string>(101, null, "参数content不能为空"));
                }

                #region 图片解析与上传
                try
                {
                    Regex re = new Regex("<img( ||.*?)src=('|\"|)([^\"|^\']+)('|\"|>| )", RegexOptions.IgnoreCase);
                    MatchCollection matches = re.Matches(content);
                    foreach (Match mh in matches)
                    {
                        string base64Str = mh.Groups[3].Value;//src里面的路径
                        if (!string.IsNullOrWhiteSpace(base64Str) && base64Str.StartsWith("data:image/"))
                        {
                            string imageData = base64Str.Split(',')[1];

                            var fileName = string.Format("{0}.jpg", Guid.NewGuid().ToString());
                            var filePath = string.Format("Upload/images/{0}", Guid.NewGuid().ToString("N").CutString(2).ToLower());
                            var serverPath = Server.MapPath("~/" + filePath);
                            byte[] imageBytes = Convert.FromBase64String(imageData);

                            if (!System.IO.Directory.Exists(serverPath))
                            {
                                System.IO.Directory.CreateDirectory(serverPath);
                            }

                            using (MemoryStream ms = new MemoryStream(imageBytes))
                            {
                                Bitmap bmp = new Bitmap(ms);
                                bmp.Save(serverPath + "/" + fileName);
                            }
                            content = content.Replace(base64Str, string.Format("http://{0}/{1}/{2}", Request.Url.Authority, filePath, fileName));
                        }
                    }
                }
                catch (Exception e)
                {
                    return Json(new ReturnResult<string>(102, null, "数据解析错误," + e.ToString()));
                }
                #endregion

                ArticleContentDataContract aritcle = new ArticleContentDataContract();
                aritcle.CategoryId = categoryId;
                aritcle.Title = title;
                aritcle.UserKeyId = token.UserKeyId;
                aritcle.Author = token.User.NickName ?? token.User.UserName;
                aritcle.ArticleContent = content;

                return Json(articleContentService.PublishArticle(aritcle));
            }
            catch (Exception e)
            {
                return Json(new ReturnResult<string>(103, null, "数据解析错误," + e.ToString()));
            }
        }

        /// <summary>
        ///  顶踩帖子
        /// </summary>
        /// <param name="type">2:鲜花(顶) 3:鸡蛋(踩) 6:举报</param>
        [OauthAuthorize]
        public JsonResult SetCount(OauthToken token, long articleId, int articleCategoryId, int type)
        {
            if (type != 2 && type != 3 && type != 6)
            {
                return Json(new ReturnResult<bool>(1, false, "参数type错误"));
            }
            return Json(articleContentService.UpdateArticleCount(articleId, articleCategoryId, token.UserKeyId, (ArticleCountEnum)type));
        }
        #endregion

        #region 评论相关接口
        /// <summary>
        /// 获取评论
        /// </summary>
        public JsonResult GetCommentList(long articleId, int page = 1, int pageSize = PAGESIZE)
        {
            return Json(articleCommentService.GetArticleCommentList(articleId, page, pageSize));
        }

        /// <summary>
        /// 发布评论
        /// </summary>
        [OauthAuthorize, HttpPost, ValidateInput(false)]
        public JsonResult PublishComment(OauthToken token, long articleId, int articleCategoryId, string content)
        {
            var comment = new ArticleCommentDataContract()
            {
                ArticleID = articleId,
                CommentContent = System.Web.HttpUtility.UrlDecode(content)
            };
            comment.UserKeyId = token.UserKeyId;
            return Json(articleCommentService.InsertArticleComment(comment, articleCategoryId));
        }
        #endregion

        #region 关注作者
        /// <summary>
        /// 获取我的关注作者
        /// </summary>
        [OauthAuthorize]
        public JsonResult GetAttention(OauthToken token, long userKeyId, int page = 1, int pageSize = PAGESIZE)
        {
            return Json(articleAttentionService.GetArticleAttention(userKeyId, page, pageSize));
        }

        /// <summary>
        /// 添加关注
        /// </summary>
        [OauthAuthorize]
        public JsonResult AddAttention(OauthToken token, long attentionUserKeyId)
        {
            var model = new ArticleAttentionDataContract();
            model.UserKeyId = token.UserKeyId;
            model.AttentionUserKeyId = attentionUserKeyId;
            model.AttentionTime = DateTime.Now;

            return Json(articleAttentionService.AddArticleAttention(model));
        }
        #endregion

        #region 收藏帖子
        /// <summary>
        /// 添加收藏
        /// </summary>
        [OauthAuthorize]
        public JsonResult CollectArticle(OauthToken token, long articleId, int categoryId)
        {
            var model = new ArticleCollectDataContract()
            {
                UserKeyId = token.UserKeyId,
                ArticleID = articleId,
                CollectTime = DateTime.Now
            };

            return Json(articleCollectService.AddArticleCollect(model, categoryId), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取收藏的帖子
        /// </summary>
        [OauthAuthorize]
        public JsonResult GetCollectArticle(OauthToken token, int page = 1, int pageSize = PAGESIZE)
        {
            return Json(articleCollectService.GetArticleCollect(token.UserKeyId, page, pageSize), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取收藏的帖子数量
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [OauthAuthorize]
        public JsonResult GetCollectArticleCount(OauthToken token)
        {
            return Json(articleCollectService.GetArticleCollectCount(token.UserKeyId), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除收藏的帖子
        /// </summary>
        /// <param name="token"></param>
        /// <param name="collectId"></param>
        /// <returns></returns>
        [OauthAuthorize]
        public JsonResult DeleteCollect(OauthToken token, long collectId)
        {
            return Json(articleCollectService.DeleteArticleCollect(token.UserKeyId, collectId), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 分类列表
        [HttpGet]
        public JsonResult GetCategorys()
        {
            return Json(new ReturnResult<List<ArticleCategoryDataContract>>(this.articleCategoryService.GetWapArticleCategoryList()));
        }
        #endregion

        #region 上传图片
        //[HttpPost]
        //public JsonResult UploadImage(OauthToken token)
        //{
        //    if (Request.Files.Count == 0 || Request.Files[0].ContentLength == 0)
        //    {
        //        return Json(new ReturnResult<string>(101, null, "未找到文件"));
        //    }
        //    var file = Request.Files[0];

        //    //Image originalImage = Image.FromStream(file.InputStream);
        //    //string base64String = "";
        //    //using (MemoryStream ms = new MemoryStream())
        //    //{
        //    //	originalImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
        //    //	byte[] imageBytes = ms.ToArray();
        //    //	base64String = Convert.ToBase64String(imageBytes);
        //    //}
        //    if (file.CheckType())
        //    {
        //        return Json(new ReturnResult<string>(102, null, "文件格式不正确，只能上传.jpg、.gif、.png格式的图片文件。"));
        //    }

        //    var dir = string.Format("Upload/images/{0}", Guid.NewGuid().ToString("N").CutString(2).ToLower());

        //    ImageSize img = file.SaveAvatarPhoto(dir);

        //    return Json(new ReturnResult<string>(0, string.Format("http://{0}/{1}_{2}.jpg", Request.Url.Authority, img.name, "600x300"), "success"));
        //}

        [HttpPost]
        public JsonResult UploadImage(OauthToken token, string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content))
                {
                    return Json(new ReturnResult<string>(101, null, "参数content不能为空"));
                }
                if (!content.StartsWith("data:image/"))
                {
                    return Json(new ReturnResult<string>(102, null, "参数content格式错误"));
                }
                string imageData = content.Split(',')[1];

                var fileName = string.Format("{0}.jpg", Guid.NewGuid().ToString());
                var filePath = string.Format("Upload/images/{0}", Guid.NewGuid().ToString("N").CutString(2).ToLower());
                var serverPath = Server.MapPath("~/" + filePath);
                byte[] imageBytes = Convert.FromBase64String(imageData);

                if (!System.IO.Directory.Exists(serverPath))
                {
                    System.IO.Directory.CreateDirectory(serverPath);
                }

                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    Bitmap bmp = new Bitmap(ms);
                    bmp.Save(serverPath + "/" + fileName);
                }
                string url = string.Format("http://{0}/{1}/{2}", Request.Url.Authority, filePath, fileName);

                return Json(new ReturnResult<string>(0, url, "success"));
            }
            catch (Exception e)
            {
                return Json(new ReturnResult<string>(103, null, "数据解析错误," + e.ToString()));
            }
        }
        #endregion
    }
}
