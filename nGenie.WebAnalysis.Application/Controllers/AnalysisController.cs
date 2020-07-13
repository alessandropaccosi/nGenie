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


using System.Data;
using System.Drawing;
//using System.Windows.Forms:
//using System.XML;
using System.Xml.Linq;
//using WindowsBase;

namespace nGenie.WebAnalysis.Application.Controllers
{

    [UserAuthorized]
    public class AnalysisController : BaseController
    {
        // GET: Analysis
        public ActionResult Index()
        {
            ActionResult result;
            ViewBag.Title = "Analysis";
            ViewBag.PageTitle = "Analysis";
            ViewBag.PageName = "nGenie Analysis";
            ViewBag.PageLink = "/Analysis";

            if (TempData["CuboId"] != null) {
                ViewBag.CuboId = (int) TempData["CuboId"];
            }

            if (TempData["ReportId"] != null)
            {
                ViewBag.ReportId = (int)TempData["ReportId"];
            }

            try
            {
                //Questa istruzione è presente perche' il codice precedente effettuava tale operazione
                //Sessione.ImpostaSessioneIfNotExistsAndUserIsValid();
                AnalysisUtente SituazioneUtente = new AnalysisUtente();
                if (SituazioneUtente.ListaDatabase.Length == 0)
                {
                    throw new UserException("Utente non abilitato su nessun database. Per l'abilitazione rivolgersi al supporto tecnico.");
                }
                else
                {
                    ViewBag.ListaDatabase = SituazioneUtente.ListaDatabase;
                    result = View(SituazioneUtente.CategorieFiltri);
                }
            }
            catch(Exception ex)
            {
                GestisciEccezione(ex);
                SetErrorMesssageForView(ExceptionUtility.GetFriendlyMessage(ex));
                result = RedirectToAction("Index", "Home");
            }

            return result;
        }

        ///// <summary>
        ///// Permette di richiamare la pagina index chiedendo di caricare un Report condiviso da un utente.
        ///// </summary>
        ///// <param name="id">Identificativo del Report condiviso</param>
        ///// <returns></returns>
        //public ActionResult IndexReportCondiviso(int id)
        //{
        //    TempData["ReportCondivisoId"] = id;

        //    //Senza questa operazione rimarrebbe l'identificativo del cubo nel parametro della url.
        //    return RedirectToAction("Index");
        //}

        /// <summary>
        /// Permette di richiamare la pagina index chiedendo di caricare il Report specificato.
        /// </summary>
        /// <param name="id">Identificativo del Report</param>
        /// <returns></returns>
        public ActionResult IndexReportId(int id)
        {
            TempData["ReportId"] = id;

            //Senza questa operazione rimarrebbe l'identificativo del cubo nel parametro della url.
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Permette di richiamare la pagina index chiedendo di passare al browser l'identificativo del cubo specificato.
        /// Utile ad esempio dopo aver eliminato un Report in modo da avere l'identificativo del cubo su cui riposizionarsi
        /// </summary>
        /// <param name="id">Identificativo del cubo da restituire al caricamento della pagina</param>
        /// <returns></returns>
        public ActionResult IndexSetCuboId(int id)
        {
            TempData["CuboId"] = id;

            //Senza questa operazione rimarrebbe l'identificativo del cubo nel parametro della url.
            return RedirectToAction("Index");
        }

        ///// <summary>
        ///// Permette di richiamare la pagina index chiedendo di passare al browser l'identificativo del Report specificato.
        ///// Utile ad esempio per ricaricare la pagina e posizionarsi su uno specifico Report.
        ///// </summary>
        ///// <param name="id">Identificativo del Report da restituire al caricamento della pagina</param>
        ///// <returns></returns>
        //public ActionResult IndexSetReportId(int id)
        //{
        //    TempData["ReportCondivisoId"] = id;

        //    //Senza questa operazione rimarrebbe l'identificativo del Report nel parametro della url.
        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        [WebMethod]
        public JsonResult SalvaFiltroUtente(String _par)
        {
            //SalvaFiltroUtenteResult result = new SalvaFiltroUtenteResult();
            EsitoPair<FiltroReference> result = new EsitoPair<FiltroReference>();
            result.EsitoOperazione.Esito = EsitoOperazioneType.ERROR;
            result.Dati = new FiltroReference();
            try
            {
                FiltroUtente report = toFiltroUtente(_par);
                int[] condividiConUtenti = getCondividiConUtenti(_par);

                FiltroRicercaReport filtroRicercaReport = new FiltroRicercaReport();
                filtroRicercaReport.Nome = report.Nome;
                filtroRicercaReport.CuboId = report.FK_CategoriaId;
                filtroRicercaReport.UtenteId = report.FK_UtenteId;
                filtroRicercaReport.Attivo = true;

                if (report.PKFiltroId == 0)
                {
                    //Richiesta: inserimento Report
                    if (ReportRepository.Exists(filtroRicercaReport))
                    {
                        result.EsitoOperazione.Messaggio = "Report già esistente. Inserire un nome diverso oppure attivare il flag Sovrascrivi il report corrente.";
                    }
                    else
                    {
                        result.Dati.FiltroId = ReportRepository.Insert(report, condividiConUtenti);

                        result.EsitoOperazione.Esito = EsitoOperazioneType.OK;
                    }
                }
                else
                {
                    //Richiesta: modifica Report
                    checkBeforeUpdateReport(report.PKFiltroId);
                    result.Dati.FiltroId = report.PKFiltroId;
                    ReportRepository.Update(report, condividiConUtenti);
                    result.EsitoOperazione.Esito = EsitoOperazioneType.OK;
                }

                //if (result.EsitoOperazione.Esito == EsitoOperazioneType.OK)
                //{
                //   abilitaDisabilitaReportPredefinito(report.FK_UtenteId, report.FK_CategoriaId);
                //}
            }
            catch (Exception ex)
            {
                GestisciEccezione(ex);
                result.EsitoOperazione.Messaggio = ExceptionUtility.GetFriendlyMessage(ex);
            }

            return Json(result);
        }

        /// <summary>
        /// Restituisce i dati del Report specificato oppure del Report piu' recente sul cubo specificato dell'utente che ha effettuato la richiesta.
        /// Saranno considerato soltanto filtri attivi.
        /// </summary>
        /// <param name="_par">Oggetto json
        ///     ReportId 
        ///         0   nessun Report specifico da caricare, la ricerca sara' effettuata
        ///         valorePositivo  indica di caricare il Report corrispondente all'identificativo specificato
        ///         valoreNegativo  indica di caricare un nuovoReport. Ad esempio ReportId=-5 indica di caricare
        ///             un nuovo Report per il Cubo con identificativo 5.
        ///     CuboId
        ///             Serve per caricare l'ultimo Report appartente al Cubo specificato.
        ///             Ad esempio ReportId=0, CuboId=5 
        ///                 Carica l'ultimo Report del Cubo con identificativo 5.
        /// </param>
        /// <returns>Il filtro più recente corrispondente ai parametri specificati</returns>
        [HttpPost]
        [WebMethod]
        public JsonResult GetReport(String _par)
        {
            EsitoPair<MyFiltro> result = new EsitoPair<MyFiltro>();
            MyFiltro retFiltro = new MyFiltro();
            try
            {
                GetUltimoFiltroParameter parameter = toGetUltimoFiltroParameter(_par);

                if (parameter.ReportId != 0)
                {
                    result.Dati = loadReport(parameter.ReportId);
                }
                else
                {
                    bool isAdministrator = Sessione.Dati.IsAdministrator;
                    using (var ctx = new TempCaringServiceEntities())
                    {
                        int idUtente = Sessione.Dati.UserId;
                        var filtro = (
                            from
                                Report in ctx.FiltroUtente
                            join Cubi in ctx.CategoriaFiltro
                                on Report.FK_CategoriaId equals Cubi.PK_CategoriaFiltroId
                            join Database in ctx.DatabasesOlap
                                on Cubi.DatabaseOlapId equals Database.Id
                            join Server in ctx.ServerOlap
                                 on Database.ServerOlapId equals Server.Id
                            join Utenti in ctx.Utente
                                on Report.FK_UtenteId equals Utenti.PkUtenteId
                            from
                                Autorizzazioni in ctx.AutorizzazioniCubi
                            where
                                    (
                                        (isAdministrator || Report.FK_UtenteId == Sessione.Dati.UserId)
                                            &&
                                        (Utenti.Abilitato)
                                            &&
                                        (Report.Attivo)
                                            &&
                                        (Cubi.Attivo)
                                            &&
                                        (Database.Attivo)
                                            &&
                                        (Server.Attivo)
                                            &&
                                        (isAdministrator || Report.FK_UtenteId == Autorizzazioni.UtenteId)
                                            &&
                                        (isAdministrator || Autorizzazioni.CuboId == Cubi.PK_CategoriaFiltroId)
                                            &&
                                        (parameter.CuboId <= 0 || Cubi.PK_CategoriaFiltroId == parameter.CuboId)
                                            &&
                                        (!Report.Condiviso)
                                    )
                            orderby
                                Report.DataUltimaModifica descending
                            select
                                Report).FirstOrDefault<FiltroUtente>();

                        //Nel caso non esista nessun filtro salvato viene restituito un nuovo report utilizzando
                        //il primo Cubo utilizzabile
                        if (filtro == null)
                        {
                            CategoriaFiltro cubo;
                            if (isAdministrator)
                            {
                                //Individua il primo Cubo disponibile
                                FiltroRicercaCubo filtroRicercaCubo = new FiltroRicercaCubo();
                                filtroRicercaCubo.Attivo = true;
                                cubo = CubiRepository.Search(filtroRicercaCubo).FirstOrDefault();
                                if (cubo == null)
                                    throw new UserException(Costanti.MSG_NESSUN_CUBO_ATTIVO);
                            }
                            else
                            {
                                //Individua il primo Cubo disponibile a cui l'utente e' autorizzato
                                AutorizzazioneCubo autorizzazioneCubo = AutorizzazioniCubiRepository.Load(idUtente).FirstOrDefault();
                                if (autorizzazioneCubo == null)
                                    throw new UserException(Costanti.MSG_NESSUN_CUBO_AUTORIZZATO);

                                cubo = CubiRepository.Load(autorizzazioneCubo.CuboId);
                            }

                            filtro = MyCategoriaFiltro.GetNuovoReport(cubo);
                        }

                        result.Dati = new MyFiltro(filtro);
                    }
                }

                result.EsitoOperazione.Esito = EsitoOperazioneType.OK;
            }
            catch (Exception ex)
            {
                GestisciEccezione(ex);
                result.EsitoOperazione.Messaggio = ExceptionUtility.GetFriendlyMessage(ex);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [WebMethod]
        public JsonResult RimuoviFiltro(int _id)
        {
            EsitoPair<FiltroReference> result = new EsitoPair<FiltroReference>();
            result.Dati = new FiltroReference();
            result.Dati.FiltroId = _id;
            try
            {
                using (var ctx = new TempCaringServiceEntities())
                {
                    FiltroUtente report = ctx.FiltroUtente.Find(_id);
                    if (report != null)
                    {
                        checkBeforeRemoveReport(report);

                        //La rimozione del filtro e' solo logica, viene solo impostato che il Report non e' piu' attivo
                        report.Attivo = false;
                        ctx.SaveChanges();

                        //AutorizzazioniCubiRepository.InsertReportPredefinitoIfNotExists(report.FK_UtenteId, report.FK_CategoriaId);
                        //abilitaDisabilitaReportPredefinito(report.FK_UtenteId, report.FK_CategoriaId);
                        result.EsitoOperazione.Esito = EsitoOperazioneType.OK;
                    }
                }
            }
            catch (Exception ex)
            {
                GestisciEccezione(ex);
                result.EsitoOperazione.Messaggio = ExceptionUtility.GetFriendlyMessage(ex);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// Restituisce gli utenti abilitati al cubo specificato
        /// </summary>
        /// <param name="id">Identificativo del Cubo a cui il Report appartiene</param>
        /// <returns></returns>
        [HttpGet]
        [WebMethod]
        public JsonResult GetUtentiInvioReport(int id)
        {
            EsitoPair<Utente[]> result = new EsitoPair<Utente[]>();
            try
            {
                result.Dati = UtentiRepository.SearchUtentiInvioReport(Sessione.Dati.UserId, id);
                result.EsitoOperazione.Esito = EsitoOperazioneType.OK;
            }
            catch (Exception ex)
            {
                GestisciEccezione(ex);
                result.EsitoOperazione.Messaggio = ExceptionUtility.GetFriendlyMessage(ex);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Effettua i controlli previsti prima dell'eliminazione di un Report
        /// </summary>
        /// <param name="report"></param>
        /// <remarks>Attualmente viene impedita l'eliminazione di qualsiasi Report predefinito</remarks>
        private void checkBeforeRemoveReport(FiltroUtente report)
        {
            if (report == null)
                throw new UserException("Report non esistente");

            if (report.Predefinito)
                throw new UserException("Non è possibile eliminare il Report predefinito");

            if (report.FK_UtenteId != Sessione.Dati.UserId)
                throw new UserException("Eliminazione non consentita. Il Report può essere eliminato solo dal proprietario.");
        }

        private void checkBeforeUpdateReport(int reportId)
        {
            FiltroUtente report = ReportRepository.Load(reportId);
            if (report.FK_UtenteId != Sessione.Dati.UserId)
                throw new UserException("Il Report appartiene ad un altro utente e non è possibile modificarlo");
            //ReportRepository.Search
            //if (report == null)
            //    throw new UserException("Report non esistente");

            //if (report.Predefinito)
            //    throw new UserException("Non è possibile eliminare il Report predefinito");

            //if (report.FK_UtenteId != Sessione.Dati.UserId)
            //    throw new UserException("Eliminazione non consentita. Il Report può essere eliminato solo dal proprietario.");
        }

        /// <summary>
        /// Converte una string Json in un oggetto FiltroUtente
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        private FiltroUtente toFiltroUtente(string jsonString)
        {
            dynamic par = JsonConvert.DeserializeObject(jsonString);
            FiltroUtente result = new FiltroUtente();

            result.PKFiltroId = par.Id;
            result.DataCreazione = DateTime.Now;
            result.DataUltimaModifica = DateTime.Now;
            result.FK_CategoriaId = par.IdCategoria;
            result.FK_UtenteId = Sessione.Dati.UserId;

            try
            {
                result.Nome = par.Nome;
            }
            catch(Exception ex)
            {
                throw new UserException("Nome del Report non valido");
            }

            result.Colonne = par.Colonne.ToString();
            result.Righe = par.Righe.ToString();
            result.Misure = par.Misure.ToString();
            result.Transport = par.Transport.ToString();
            result.Attivo = true;
            if (par.Filtri == null)
            {
                par.Filtri = "";
            }
            if (par.Sort == null)
            {
                par.Sort = "";
            }
            result.Filtri = par.Filtri.ToString();
            result.Sort = par.Sort.ToString();

            return result;
        }

        private GetUltimoFiltroParameter toGetUltimoFiltroParameter(string jsonString)
        {
            dynamic par = JsonConvert.DeserializeObject(jsonString);
            GetUltimoFiltroParameter result = new GetUltimoFiltroParameter();

            result.CuboId = par.CuboId;
            result.ReportId = par.ReportId;

            return result;
        }



        /// <summary>
        /// Restituisce gli identificativi degli Utenti a cui inviare il Report
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        private int[] getCondividiConUtenti(string jsonString)
        {
            dynamic par = JsonConvert.DeserializeObject(jsonString);
            int[] result = par.CondividiConUtenti.ToObject<int[]>();

            return result;
        }

        //private void abilitaDisabilitaReportPredefinito(int utenteId, int cuboId)
        //{
        //    //Impostazione filtro di ricerca per individuare se l'utente ha propri report salvati
        //    FiltroRicercaReport filtroRicerca = new FiltroRicercaReport();
        //    filtroRicerca.CuboId = cuboId;
        //    filtroRicerca.UtenteId = utenteId;
        //    filtroRicerca.Attivo = true;
        //    filtroRicerca.Predefinito = false;
        //}

        private ActionResult GetSituazioneUtente()
        {
            AnalysisUtente SituazioneUtente = new AnalysisUtente();
            return PartialView("~/Views/Shared/NavBar/_leftnavbar.cshtml", SituazioneUtente.CategorieFiltri);
        }

        /// <summary>
        /// Carica i dati del Report specificato
        /// </summary>
        /// <param name="id">
        ///     Se maggiore di zero indica di caricare il Report corrispondente all'identificativo specificato
        ///     Se minore di zero indica di caricare un nuovo Report per il cubo specificato.
        ///         Ad esempio id=-5 indica di caricare i dati di un nuovo Report appartenente al Cubo 5
        /// </param>
        /// <returns></returns>
        private MyFiltro loadReport(int id)
        {
            MyFiltro result = new MyFiltro();
            FiltroUtente report = null;

            //Se l'identificativo del Report e' negativo saranno caricati i dati di un nuovo Report
            if (id < 0)
            {

                report = MyCategoriaFiltro.GetNuovoReport(-id);
                result = new MyFiltro(report);
            }
            else
            {
                //Caricamento di un normale Report dal database.
                using (var ctx = new TempCaringServiceEntities())
                {
                    // Estraggo il filtro richiesto
                    report = (from f in ctx.FiltroUtente
                                  where
                                  (
                                  (f.PKFiltroId == id)
                                      &&
                                  (Sessione.Dati.IsAdministrator || f.FK_UtenteId == Sessione.Dati.UserId || f.Condiviso)
                                  )
                                  select f).FirstOrDefault<FiltroUtente>();

                    if (report == null)
                        throw new UserException("Report non trovato o non autorizzato");

                    if (report.Condiviso)
                    {
                        ReportCondiviso reportCondiviso = ReportCondivisiRepository.GetReportCondivisoByIdReport(report.PKFiltroId);
                        report.Nome = reportCondiviso.Nome;
                    }

                    result = new MyFiltro(report);
                }
            }

            return result;
        }
    }
}
