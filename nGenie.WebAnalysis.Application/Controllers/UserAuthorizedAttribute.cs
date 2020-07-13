using nGenie.WebAnalysis.Application.Common;
using nGenie.WebAnalysis.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace nGenie.WebAnalysis.Application.Controllers
{
    public class UserAuthorizedAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (IdentityUtility.UserIsAuthenticated)
            {
                if (Sessione.IsValid)
                    return true;
                else
                    try
                    {
                        //Prova ad impostare la sessione
                        Sessione.ImpostaSessioneIfNotExistsAndUserIsValid();
                    }
                    catch(UtenteNonAutorizzatoException)
                    {
                        return false;
                    }
                    catch(Exception ex)
                    {
                        BaseController.GestisciEccezione(ex);
                        //result = GetActionResultErrorRedirect(ExceptionUtility.GetFriendlyMessage(ex));
                        return false;
                    }
            }

            return base.AuthorizeCore(httpContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //Se non viene chiamata anche l'utente non autorizzato può chiamare metodi con attributo UserAuthorizedAttribute
            base.HandleUnauthorizedRequest(filterContext);

            System.Web.Routing.RouteValueDictionary route = new System.Web.Routing.RouteValueDictionary(
                    new { controller = "Info", action = "AccessoNegato" }
                    );

            filterContext.Result = new RedirectToRouteResult(route);
        }
    }
}