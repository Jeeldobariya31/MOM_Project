using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MOM.Filters
{
    public class AuthenticationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var userID = session.GetString("UserID");

            // Skip authentication for Auth controller
            var controller = context.RouteData.Values["controller"]?.ToString();
            var action = context.RouteData.Values["action"]?.ToString();

            if (controller?.Equals("Auth", StringComparison.OrdinalIgnoreCase) == true)
            {
                base.OnActionExecuting(context);
                return;
            }

            // If user is not logged in, redirect to login
            if (string.IsNullOrEmpty(userID))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}