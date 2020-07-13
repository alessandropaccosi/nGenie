using nGenie.WebAnalysis.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace nGenie.WebAnalysis.Application.Controllers
{
    /// <summary>
    /// Controller per riportare messaggi di errore
    /// </summary>
    public class InfoController : Controller
    {
        public ActionResult MostraMessaggio(string messaggio)
        {
            ViewBag.Message = messaggio;
            return View("ShowMessage");
        }

        /// <summary>
        /// Mostra un tooltip contenente il messaggio scritto in TempData[Costanti.KEY_ERROR_MESSAGE];
        /// </summary>
        /// <returns></returns>
        // GET: Info
        public ActionResult Index()
        {
            return View();
        }

        //// GET: Info
        //public ActionResult ShowMessage()
        //{
        //    ViewBag.Message = "Messaggio";
        //    return View();
        //}

        /// <summary>
        /// Mostra un messaggio per informare l'utente che non è autorizzato all'utilizzo dell'applicazione.
        /// </summary>
        /// <returns></returns>
        // GET: Info
        public ActionResult AccessoNegato()
        {
            ViewBag.Message = "Utente non autorizzato";
            return View("ShowMessage");
        }

        /// <summary>
        /// Mostra un messaggio per informare che l'utente e' stato disconnesso.
        /// </summary>
        /// <returns></returns>
        // GET: Info
        public ActionResult LogOff()
        {
            Sessione.Abandon();
            ViewBag.Message = "Utente disconnesso. Chiudere la finestra del browser.";
            return View("ShowMessage");
        }
    }
}