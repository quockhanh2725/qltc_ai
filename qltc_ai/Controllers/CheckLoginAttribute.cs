using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace qltc_ai.Controllers
{
    public class CheckLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var accId = context.HttpContext.Session.GetInt32("AccountId");

            if (accId == null)
                //context.Result = new UnauthorizedObjectResult("Vui long dang nhap");
                context.Result = new RedirectToActionResult("", "account", null);
        }
    }
}
