using Microsoft.Extensions.Caching.Memory;
using qltc_ai.Service.Base;

namespace qltc_ai.Middlewares
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;

        public AuthorizationMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context, IAccountService acc)
        {
            var path = context.Request.Path.Value?.ToLower();

            if (path != null && path.StartsWith("/admin"))
            {
                var accId = context.Session.GetInt32("AccountId");

                
                if (accId == null)
                {
                    context.Response.Redirect("/account");
                    return;
                }

               
                var loginMonth = context.Session.GetInt32("LoginMonth");
                var loginYear  = context.Session.GetInt32("LoginYear");
                var now        = DateTime.Now;

                if (loginMonth == null || loginYear == null
                    || now.Month != loginMonth.Value
                    || now.Year  != loginYear.Value)
                {
                    context.Session.Clear();
                    context.Response.Redirect("/account");
                    return;
                }

                
                var cacheKey = $"role_{accId.Value}";
                if (!_cache.TryGetValue(cacheKey, out int cachedRole))
                {
                    var accC = acc.GetAccountById(accId.Value);
                    if (accC == null)
                    {
                        context.Session.Clear();
                        context.Response.Redirect("/account");
                        return;
                    }
                    cachedRole = accC.RoleId.Value;
                    _cache.Set(cacheKey, cachedRole, TimeSpan.FromMinutes(5));
                }

               
                if (cachedRole != 1)
                {
                    context.Response.Redirect("/account");
                    return;
                }
            }

            await _next(context);
        }
    }
}
