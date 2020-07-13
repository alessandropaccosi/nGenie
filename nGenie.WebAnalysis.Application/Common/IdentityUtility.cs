using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace nGenie.WebAnalysis.Application.Common
{
    public class IdentityUtility
    {
        public static bool UserIsAuthenticated
        {
            get
            {
                return System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            }
        }

        public static string UsernameWithoutDomain
        {
            get
            {
                return StringUtility.GetUsernameWithoutDomain(System.Web.HttpContext.Current.User.Identity.Name);
            }
        }

        /// <summary>
        /// Restituisce true se l'utente e' autenticato e l'identità dell'utente è valida
        /// </summary>
        public static bool IsValid
        {
            get
            {
                return
                    UserIsAuthenticated
                        &&
                    !String.IsNullOrEmpty(UsernameWithoutDomain);
            }
        }
    }
}