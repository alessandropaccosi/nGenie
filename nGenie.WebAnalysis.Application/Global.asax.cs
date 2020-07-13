using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using nGenie.WebAnalysis.Application.Common;
using nGenie.WebAnalysis.Application.Controllers;
using nGenie.WebAnalysis.Application.Models;


namespace nGenie.WebAnalysis.Application
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Session_Start(Object sender, EventArgs e)
        {
            //try
            //{
            //    if (IdentityUtility.IsValid)
            //    {
            //        Sessione.ImpostaSessioneIfNotExistsAndUserIsValid();
            //    }
            //}
            //catch (UtenteNonAutorizzatoException ex)
            //{
            //    Response.Write(ex.Message);
            //    Response.StatusCode = 401;
            //    Response.End();
            //}
        }

        protected void Session_End(Object sender, EventArgs E)
        {
            // Clean up session resources
        }

        protected void Application_Start()
        {
            //***abilitare
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            //HttpCookie authCookie = Request.Cookies["Cookie1"];
            //if (authCookie != null)
            //{
            //    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

            //    var serializeModel = JsonConvert.DeserializeObject<CustomSerializeModel>(authTicket.UserData);

            //    CustomPrincipal principal = new CustomPrincipal(authTicket.Name);

            //    principal.UserId = serializeModel.UserId;
            //    principal.FirstName = serializeModel.FirstName;
            //    principal.LastName = serializeModel.LastName;
            //    principal.Roles = serializeModel.RoleName.ToArray<string>();

            //    HttpContext.Current.User = principal;
            //}
        }

        /// <summary>
        /// Serve per comunicare ad una richiesta Ajax che la sessione e' terminata.
        /// Interviene solo se la richiesta richiede un json come risposta.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //protected void Application_PostAcquireRequestState(Object sender, EventArgs e)
        //{
        //    string path = Request.Path;
        //    if (
        //            !path.Contains("Account")
        //                &&
        //            (
        //                !Sessione.Esiste
        //                    ||
        //                !System.Web.HttpContext.Current.User.Identity.IsAuthenticated
        //            )
        //        )
        //    {
        //        if (isJsonRequest())
        //        {
        //            Response.Write(
        //                JsonConvert.SerializeObject(
        //                    new
        //                    {
        //                        EsitoOperazione =
        //                            new
        //                            {
        //                                Codice = Costanti.KEY_ERROR_SESSIONE_SCADUTA,
        //                                Esito = 1,
        //                                Messaggio = Costanti.MSG_SESSIONE_SCADUTA
        //                            }
        //                    }
        //                )
        //            );

        //            //Istruzione necessaria per impedire di chiamare ulteriori metodi.
        //            Response.End();
        //        }
        //    }
        //}

        private bool isJsonRequest()
        {
            var acceptTypes = Request.AcceptTypes;
            return acceptTypes != null &&
                   acceptTypes.Any(a => a.Equals("application/json",
                        StringComparison.OrdinalIgnoreCase));
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex != null)
            {

                Response.Clear();
                Server.ClearError();
                LogUtility.InsertLogWithoutException(ex);

                //var routeData = new RouteData();
                //routeData.Values["controller"] = "Home";
                //routeData.Values["action"] = "Index";
                //Response.StatusCode = 500;

                //IController controller = new InfoController();
                //var rc = new RequestContext(new HttpContextWrapper(Context), routeData);
                //controller.Execute(rc);

                //string message = ExceptionUtility.GetFriendlyMessage(ex);
                // Mostrare il messaggio
            }
        }
    }


    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    //public class MyAuthorizeAttribute : AuthorizeAttribute
    //{
    //    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    //    {
    //        if (filterContext.HttpContext.Request.IsAjaxRequest())
    //        {
    //            filterContext.Result = new JsonResult
    //            {
    //                Data = new
    //                {
    //                    // put whatever data you want which will be sent
    //                    // to the client
    //                    message = "sorry, but you were logged out"
    //                },
    //                JsonRequestBehavior = JsonRequestBehavior.AllowGet
    //            };
    //        }
    //        else
    //        {
    //            base.HandleUnauthorizedRequest(filterContext);
    //        }
    //    }
    //}
}
