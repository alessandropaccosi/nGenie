using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using Newtonsoft.Json;
using nGenie.WebAnalysis.Application.Models;
using nGenie.WebAnalysis.Application.DataAccess;
using nGenie.WebAnalysis.Application.DataAccess.Interfaces;
using nGenie.WebAnalysis.Application.DataAccess.Repository;
using nGenie.WebAnalysis.Application.Models.DbModel;
using Microsoft.AspNet.Identity;
using nGenie.WebAnalysis.Application.Common;

namespace nGenie.WebAnalysis.Application.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// Restituisce la ActionResult per effettuare una redirect su Home o Account e mostrare il messaggio di errore specificato.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        //public ActionResult RedirectToActionErrorMessage(string errorMessage)
        //{
        //    SetErrorMesssageForView(errorMessage);
        //    if (Sessione.IsValid)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    else
        //        return RedirectToAction("Index", "Info");
        //}

        /// <summary>
        /// Gestisce l'eccezione specificata scrivendola nel log se necessario.
        /// </summary>
        /// <param name="ex"></param>
        public static void GestisciEccezione(Exception ex)
        {
            bool inserisciLog = true;
            if (ex is UserException)
            {
                inserisciLog = false;
            }

            if (inserisciLog)
                LogUtility.InsertLogWithoutException(ex);
        }

        /// <summary>
        /// Imposta il messaggio di errore in modo che possa essere letto da una vista.
        /// Viene usata la struttura dati TempData in quanto continua ad esistere anche dopo una Redirect.
        /// Attualmente il messaggio di errore viene gestito dalla seguente vista condivisa: view/shared/_Layout.cshtml 
        /// </summary>
        /// <param name="errorMessage"></param>
        public void SetErrorMesssageForView(string errorMessage)
        {
            TempData[Costanti.KEY_ERROR_MESSAGE] = errorMessage;
        }

        //public static void ShowMessage(string message)
        //{
        //    System.Web.HttpContext.Current.Response.ClearHeaders();
        //    System.Web.HttpContext.Current.Response.ContentType = "text/html";
        //    //System.Web.HttpContext.Current.Response.BufferOutput = true;
        //    //TempData[Costanti.KEY_ERROR_MESSAGE] = errorMessage;

        //    //RedirectToAction("yourActionName", "YourControllerName");
            
        //    //Html.Action("PkRkAction", new { pkrk = new PkRk { pk = 400, rk = 500 } })

        //    //System.Web.HttpContext.Current.Server.Transfer("/Info/ShowMessage");
            
        //}
    }
}
