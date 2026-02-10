using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Resonance.PresentationLayer.Filters
{
    /// <summary>
    /// Custom authorization filter voor sessie-gebaseerde authenticatie
    /// </summary>
    public class SessionAuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userId = context.HttpContext.Session.GetInt32("UserId");
            
            if (userId == null)
            {
                // Redirect naar login als niet ingelogd
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }
        }
    }
}

