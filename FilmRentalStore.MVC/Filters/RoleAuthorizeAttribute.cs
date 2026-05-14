using FilmRentalStore.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FilmRentalStore.MVC.Filters
{
    public class RoleAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string[] _allowedRoles;

        public RoleAuthorizeAttribute(params string[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetString(SessionKeys.Role);
            var token = context.HttpContext.Session.GetString(SessionKeys.JwtToken);

            if (string.IsNullOrWhiteSpace(token))
            {
                context.Result = new RedirectToActionResult("StaffLogin", "Auth", null);
                return;
            }

            if (string.IsNullOrWhiteSpace(role) || !_allowedRoles.Contains(role))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Error", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
