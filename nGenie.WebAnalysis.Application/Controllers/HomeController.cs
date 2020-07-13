using nGenie.WebAnalysis.Application.Models;
using System;
using System.Configuration;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Net;
using System.Web.SessionState;
using nGenie.WebAnalysis.Application.Common;
using nGenie.WebAnalysis.Application.Models.DbModel;
using Newtonsoft.Json;
using nGenie.WebAnalysis.Application.DataAccess.Repository;
using System.Web.Services;

namespace nGenie.WebAnalysis.Application.Controllers
{
    [UserAuthorized]
    public class HomeController : BaseController
    {
        //protected static string applicationName = ConfigurationManager.AppSettings["ApplicationName"];
        //protected static string rootDomain = ConfigurationManager.AppSettings["RootDomain"];

        public ActionResult Index()
        {
            try
            {
                ViewBag.IsAdministrator = Sessione.Dati.IsAdministrator;
                ViewBag.PageTitle = "Home";
                return View();
            }
            catch(Exception ex)
            {
                GestisciEccezione(ex);
                SetErrorMesssageForView(ExceptionUtility.GetFriendlyMessage(ex));
                return RedirectToAction("Index", "Info");
            }
        }

        /// <summary>
        /// Permette di inserire un log
        /// </summary>
        /// <param name="_par">Oggetto Json corrispondente al tipo di dato Log</param>
        /// <returns></returns>
        [HttpPost]
        [WebMethod]
        public JsonResult InserisciLog(string _par)
        {
            EsitoPair<int> result = new EsitoPair<int>();
            try
            {
                Log log = toLog(_par);
                LogRepository.Insert(log);
                result.EsitoOperazione.Esito = EsitoOperazioneType.OK;
            }
            catch (Exception ex)
            {
                GestisciEccezione(ex);
                result.EsitoOperazione.Messaggio = ExceptionUtility.GetFriendlyMessage(ex);
            }

            return Json(result);
        }

        /// <summary>
        /// Converte una stringa json in un oggetto Log
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        private Log toLog(string jsonString)
        {
            dynamic par = JsonConvert.DeserializeObject(jsonString);
            Log result = new Log();

            if (par.Messaggio != null)
                result.Messaggio = par.Messaggio;

            if (par.StackTrace != null)
                result.StackTrace = par.StackTrace;

            return result;
        }

        //Metodo disabilitato se serve riabilitare
        //[HttpPost]
        //public ActionResult Index(LoginInfoViewModel info)
        //{
        //    Session["UserId"] = info.UserName;

        //    return RedirectToAction("UserSessionSection");
        //}

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}

        //Metodo disabilitato, riabilitarlo se serve
        //public ActionResult UserSessionSection()
        //{
        //    var Data_session = new LoginInfoViewModel();
        //    try
        //    {
        //        if ((Object)Session["UserId"] != null) Data_session.Session_Val = "Hello " + Session["UserId"].ToString();
        //        else Data_session.Session_Val = "Session has been expired";
        //    }
        //    catch { }
        //    return View(Data_session);
        //}

        //public object GetSession(ASPStateTempSessions sentity, string sessionkey)
        //{
        //    object obj = null;

        //    System.IO.MemoryStream stream = new System.IO.MemoryStream();
        //    System.IO.BinaryReader reader = new System.IO.BinaryReader(stream);
        //    stream.SetLength(0);

        //    if (sentity.SessionItemShort != null)
        //    {
        //        var _len = ;
        //        stream.Write(sentity.SessionItemShort, 0, sentity.SessionItemShort.Length);
        //    }
        //    else
        //    {
        //        stream.Write(sentity.SessionItemLong, 0, sentity.SessionItemLong.Length);
        //    }

        //    stream.Seek(0, System.IO.SeekOrigin.Begin);
        //    reader.ReadInt32();
        //    bool bol_flag = reader.ReadBoolean();
        //    reader.ReadBoolean();
        //    if (bol_flag)
        //    {
        //        System.Web.SessionState.SessionStateItemCollection sessionItems = System.Web.SessionState.SessionStateItemCollection.Deserialize(reader);
        //        foreach (string key in sessionItems.Keys) //sessionidsession  
        //        {
        //            if (sessionkey.Equals(key))
        //            {
        //                obj = sessionItems[key];
        //                break;
        //            }
        //        }
        //    }

        //    return obj;
        //}
    }
}