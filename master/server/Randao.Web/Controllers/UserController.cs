using Randao.Core;
using Randao.Core.Service;
using Randao.DataContracts;
using System;
using System.Drawing;
using System.IO;
using System.Web.Mvc;


namespace Randao.Web.Controllers
{
    public class UserController : CustomerController
    {
        private MemberService memberService;
        private TokenService tokenService;
        public UserController(MemberService _memberService, TokenService _tokenService)
        {
            this.memberService = _memberService;
            this.tokenService = _tokenService;
        }

        #region 测试页面
        public ActionResult RegsiterTest()
        {
            return View();
        }

        public ActionResult LoginTest()
        {
            return View();
        }
        #endregion

        /// <summary>
        /// 获取用户信息
        /// </summary>
        [OauthAuthorize]
        public JsonResult Get(OauthToken token, long userKeyId)
        {
            var user = memberService.GetMember(userKeyId);

            return Json(new ReturnResult<TdMemberDataContract>(user), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取当前登录用户信息
        /// </summary>
        [OauthAuthorize]
        public JsonResult GetCurr(OauthToken token)
        {
            return Json(new ReturnResult<TdMemberDataContract>(token.User), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        [HttpPost]
        public JsonResult Regsiter(string userName, string passWord)
        {
            var user = new TdMemberDataContract();

            user.UserName = userName;
            user.Password = passWord;
            user.RegIP = System.Web.HttpContext.Current.Request.GetIp();
            user.LastLoginIp = user.RegIP;

            var result = memberService.RegisterUser(user);

            if (result.Code != 0)
            {
                return Json(result);
            }
            return Json(tokenService.CreateAccessToken(user));
        }

        /// <summary>
        /// 申请token
        /// </summary>
        public JsonResult Token(string userName, string passWord)
        {
            var clientIp = System.Web.HttpContext.Current.Request.GetIp();
            return Json(tokenService.CreateAccessToken(userName, passWord, clientIp), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        public JsonResult refreshToken(string refreshToken, int clientId)
        {
            return Json(tokenService.RefreshAccessToken(refreshToken, clientId), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 上传用户头像
        /// </summary>
        /// <param name="token"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [HttpPost, OauthAuthorize]
        public JsonResult UploadFaceImg(OauthToken token, string content)
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

                if (memberService.UpdateFaceImg(token.UserKeyId, url))
                {
                    return Json(new ReturnResult<string>(0, url, "success"));
                }
                else
                {
                    return Json(new ReturnResult<string>(104, null, "保存失败"));
                }
            }
            catch (Exception e)
            {
                return Json(new ReturnResult<string>(103, null, "数据解析错误," + e.ToString()));
            }
        }
    }
}