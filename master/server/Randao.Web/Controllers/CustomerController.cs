using System;
using System.Web.Mvc;
using System.Text;
using Randao.Core;

namespace Randao.Web
{
    public class CustomerController : Controller
    {
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new CustomJsonResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding
            };
        }

        protected void VerifyToErrorAction(Func<bool> acquire, int errorCode, string message = "")
        {
            if (!acquire())
            {
                this.RedirectToErrorAction(errorCode, message);
            }
        }

        protected ActionResult RedirectToErrorAction(int code, string message = "")
        {
            return Redirect("/error?message=" + message);
        }
    }

    public class CustomJsonResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var response = context.HttpContext.Response;

            response.ContentType = !String.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            if (Data != null)
            {
                response.Write(JSONHelper.Encode(Data));
            }
        }
    }
}
