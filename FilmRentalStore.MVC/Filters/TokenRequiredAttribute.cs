using FilmRentalStore.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FilmRentalStore.MVC.Filters
{
    public class TokenRequiredAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Session.GetString(SessionKeys.JwtToken);

            if (string.IsNullOrWhiteSpace(token))
            {
                context.Result = new RedirectToActionResult("StaffLogin", "Auth", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}