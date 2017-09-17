//using Randao.Core.Service;
//using Randao.DataContracts;
//using Randao.Web.Controllers;
//using System.Web.Mvc;

//namespace Randao.Web
//{
//	/// <summary>
//	/// cookie认证登录方案已取消,修改成了简单oauth2协议
//	/// </summary>
//	public class LoginAuthorize : FilterAttribute, IAuthorizationFilter, IActionFilter
//	{
//		protected TdMemberDataContract User;
//		protected MemberService memberService;
//		protected bool IsMustLogin = true;

//		/// <summary>
//		/// 登录验证
//		/// </summary>
//		/// <param name="isMustLogin">是否必须登录(如果是则未登录返回对应字符串,否则user对象赋值null)</param>
//		public LoginAuthorize(bool isMustLogin = true)
//		{
//			this.IsMustLogin = isMustLogin;
//			this.memberService = DependencyResolver.Current.GetService<MemberService>();
//			User = memberService.GetCurrentLoginUser();
//		}

//		public void OnAuthorization(AuthorizationContext filterContext)
//		{
//			if (filterContext == null)
//			{
//				filterContext.Result = new CustomJsonResult() { Data = new ReturnResult(-102, "程序异常,filterContext为null!") };
//			}
//			if (IsMustLogin && User == null)
//			{
//				filterContext.Result = new CustomJsonResult() { Data = new ReturnResult(-101, "未检测到登陆用户") };
//			}
//		}

//		public void OnActionExecuted(ActionExecutedContext filterContext)
//		{
			
//		}

//		public void OnActionExecuting(ActionExecutingContext filterContext)
//		{
//			if (filterContext.ActionParameters.ContainsKey("user"))
//			{
//				filterContext.ActionParameters["user"] = User;
//			}
//		}
//	}
//}