using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Identity.Client;

namespace csharp.Attribute;

[System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public class HasSessionAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.HttpContext.Session.GetInt32("userId") == null)
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }
    }

}