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
    public class ReportCondivisiController : BaseController
    {
        // GET: ReportCondivisi
        public ActionResult Index()
        {
            ActionResult result;
            ViewBag.PageTitle = "Report Condivisi";
            ViewBag.PageName = "Report Condivisi";
            ViewBag.PageLink = "/ReportCondivisi";
            ViewBag.PaginaPrecedente = "/Home";

            try
            {
                ViewBag.IsAdministrator = Sessione.Dati.IsAdministrator;
                ReportCondivisiIndex reportInfo = new ReportCondivisiIndex();
                reportInfo.ReportInviati = ReportCondivisiRepository.SearchReportInviati(Sessione.Dati.UserId);
                reportInfo.ReportRicevuti = ReportCondivisiRepository.SearchReportRicevuti(Sessione.Dati.UserId);

                result = View(reportInfo);
            }
            catch (Exception ex)
            {
                GestisciEccezione(ex);
                SetErrorMesssageForView(ExceptionUtility.GetFriendlyMessage(ex));
                result = RedirectToAction("Index", "Home");
            }

            return result;
        }
    }
}