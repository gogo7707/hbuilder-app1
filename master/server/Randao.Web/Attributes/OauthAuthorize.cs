using Randao.Core;
using Randao.Core.Service;
using Randao.DataContracts;
using Randao.Web.Controllers;
using System;
using System.Web.Mvc;

namespace Randao.Web
{
	public class OauthAuthorize : FilterAttribute, IAuthorizationFilter, IActionFilter
	{
		protected OauthToken accessToken = null;
		protected TokenService tokenService;
		protected MemberService memberService;

		public OauthAuthorize()
		{
			this.tokenService = DependencyResolver.Current.GetService<TokenService>();
			this.memberService = DependencyResolver.Current.GetService<MemberService>();
		}

		public void OnAuthorization(AuthorizationContext filterContext)
		{
			if (filterContext == null)
			{
				filterContext.Result = new CustomJsonResult() { Data = new ReturnResult(-102, "程序异常,filterContext为null!") };
			}
			var tokenString = filterContext.HttpContext.Request["accessToken"];
			var clientId = Convert.ToInt32(filterContext.HttpContext.Request["clientId"]);

			accessToken = tokenService.GetAccessToken(tokenString);

			if (null == accessToken)
			{
				filterContext.Result = new CustomJsonResult() { Data = new ReturnResult(11, "参数assessToken错误") };
				return;
			}
			if (accessToken.ClientID != clientId)
			{
				filterContext.Result = new CustomJsonResult() { Data = new ReturnResult(12, "参数clientId错误") };
			}
			if (accessToken.Expires < DateTime.Now.Epoch())
			{
				filterContext.Result = new CustomJsonResult() { Data = new ReturnResult(13, "assessToken已过期,请刷新或者重新登录") };
			}
			accessToken.ClientIp = filterContext.HttpContext.Request.GetIp();
			accessToken.User = memberService.GetMember(accessToken.UserKeyId);
			if (null == accessToken.User)
			{
				filterContext.Result = new CustomJsonResult() { Data = new ReturnResult(14, "用户已失效,请重新登录") };
			}
		}


		public void OnActionExecuted(ActionExecutedContext filterContext)
		{

		}

		public void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (filterContext.ActionParameters.ContainsKey("token"))
			{
				filterContext.ActionParameters["token"] = accessToken;
			}
		}
	}
}