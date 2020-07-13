using nGenie.WebAnalysis.Application.DataAccess.Repository;
using nGenie.WebAnalysis.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using Newtonsoft.Json;
using nGenie.WebAnalysis.Application.Models.DbModel;
using nGenie.WebAnalysis.Application.Common;

namespace nGenie.WebAnalysis.Application.Controllers
{
    [UserAuthorized]
    public class AdminController : BaseController
    {
        /// <summary>
        /// Restituisce la Vista per la gestione amministrativa.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //@@Eliminare
            //CubiRepository.Test();

            ViewBag.IsMenuStandard = true;
            ViewBag.PageTitle = "Area Amministrazione";
            ViewBag.Sottotitolo = TITOLO_LISTA_UTENTI;

            return View(creaVociMenu());
        }

        /// <summary>
        /// Resituisce la vista parziale per la gestione dei server olap
        /// </summary>
        /// <param name="nome">Parametro di ricerca opzionale per ottenere solo database contenenti il nome specificato</param>
        /// <returns></returns>
        /// <remarks>@@Gestire eccezioni</remarks>
        public ActionResult PartialViewElencoServer(string nome)
        {
            FiltroRicercaServer filtro = new FiltroRicercaServer();
            filtro.Nome = nome;
            try
            {
                ServerOlap[] listaServer = ServerOlapRepository.Search(filtro);
                ViewBag.Sottotitolo = TITOLO_LISTA_SERVER;

                return PartialView("_ServerLista", listaServer);
            }
            catch(Exception ex)
            {
                GestisciEccezione(ex);
                SetErrorMesssageForView(ExceptionUtility.GetMessageForAdministrator(ex));

                return PartialView("_ServerLista", new ServerOlap[] { });
            }
        }

        /// <summary>
        /// Restituisce la vista parziale per l'inserimento o la modifica di server Olap.
        /// </summary>
        /// <param name="id">In caso di modifica specificare l'identificatore del server da modificare</param>
        /// <returns></returns>
        public ActionResult PartialViewInserisciModificaServer(int? id)
        {
            ServerOlap server = creaServerVuotoPerInserimento();
            if (id.HasValue)
            {
                server = ServerOlapRepository.Load(id.Value);
                ViewBag.Sottotitolo = TITOLO_MODIFICA_SERVER;
            }
            else
                ViewBag.Sottotitolo = TITOLO_NUOVO_SERVER;

            return PartialView("_ServerDettaglio", server);
        }

        /// <summary>
        /// Permette di inserire o modificare il server specificato
        /// </summary>
        /// <param name="_par">Server in formato json</param>
        /// <returns>Esito dell'operazione in formato json</returns>
        [HttpPost]
        public ActionResult InserisciModificaServer(string _par)
        {
            EsitoPair<int> result = new EsitoPair<int>();
            try
            {
                ServerOlap server = toServerOlap(_par);
                ServerOlapRepository.InsertOrUpdate(server);
                result.EsitoOperazione.Esito = EsitoOperazioneType.OK;
            }
            catch (Exception ex)
            {
                GestisciEccezione(ex);
                result.EsitoOperazione.Messaggio = ExceptionUtility.GetMessageForAdministrator(ex);
            }

            return Json(result);
        }

        /// <summary>
        /// Abilita o disabilita il server specificato. 
        /// </summary>
        /// <param name="_par">Identificatore del server e valore dell'abilitazione. Esempio:
        ///     { Id=3, Attivo=true }
        /// </param>
        /// <returns></returns>
        [HttpPost]
        [WebMethod]
        public JsonResult AbilitaDisabilitaServer(string _par)
        {
            EsitoPair<int> result = new EsitoPair<int>();
            try
            {
                ServerOlap parametro = toServerOlap(_par);
                ServerOlap server = ServerOlapRepository.Load(parametro.Id);
                server.Attivo = parametro.Attivo;
                ServerOlapRepository.Update(server);
                result.EsitoOperazione.Esito = EsitoOperazioneType.OK;
            }
            catch (Exception ex)
            {
                GestisciEccezione(ex);
                result.EsitoOperazione.Messaggio = ExceptionUtility.GetMessageForAdministrator(ex);
            }

            return Json(result);
        }

        /// <summary>
        /// Restituisce le autorizzazioni dell'utente specificato.
        /// </summary>
        /// <param name="id">Identificatore dell'utente</param>
        /// <returns></returns>
        public ActionResult PartialViewAutorizzazioniUtenteCubi(int id)
        {
            UtenteAutorizzazioniCubi autorizzazioni = new UtenteAutorizzazioniCubi();

            //Carica informazioni sull'utente
            autorizzazioni.Utente = UtentiRepository.Load(id);

            //Carica informazioni sui Server attivi
            FiltroRicercaServer filtroRicercaServer = new FiltroRicercaServer { Attivo = true };
            ServerOlap[] serverOlapAttivi = ServerOlapRepository.Search(filtroRicercaServer);

            autorizzazioni.ListaServer = removeDatabaseNonAttiviEServerVuoti(serverOlapAttivi);

            //Carica informazioni sui database attivi
            FiltroRicercaDatabase filtroRicercaDatabase = new FiltroRicercaDatabase { Attivo = true };
            autorizzazioni.ListaDatabase = DatabasesOlapRepository.Search(filtroRicercaDatabase);

            //Carica informazioni sui cubi attivi
            FiltroRicercaCubo filtroRicercaCubo = new FiltroRicercaCubo { Attivo = true };
            autorizzazioni.ListaCubi = CubiRepository.Search(filtroRicercaCubo);

            //Carica le autorizzazioni sui cubi
            autorizzazioni.ListaAutorizzazioniCubi = AutorizzazioniCubiRepository.Load(id);

            ViewBag.Sottotitolo = TITOLO_AUTORIZZAZIONI;
            return PartialView("_UtenteAutorizzazioniCubi", autorizzazioni);
        }

        [HttpPost]
        [WebMethod]
        public JsonResult SalvaAutorizzazioniUtenteCubi(string _par)
        {
            EsitoPair<int> result = new EsitoPair<int>();
            try
            {
                int utenteId = getUtenteId(_par);
                int[] cubiAutorizzati = getCubiAutorizzati(_par);

                AutorizzazioniCubiRepository.ImpostaAutorizzazioni(utenteId, cubiAutorizzati);
                result.EsitoOperazione.Esito = EsitoOperazioneType.OK;

            }
            catch (Exception ex)
            {
                GestisciEccezione(ex);
                result.EsitoOperazione.Messaggio = ExceptionUtility.GetMessageForAdministrator(ex);
            }

            return Json(result);
        }

        /// <summary>
        /// Restituisce la vista parziale per la gestione dei Cubi.
        /// </summary>
        /// <param name="id">Identificatore del database eventualmente da selezionare</param>
        /// <returns></returns>
        public ActionResult PartialViewGestioneCubi(int? id)
        {
            FiltroRicercaDatabase filtro = new FiltroRicercaDatabase { Attivo = true };
            DatabaseOlap[] listaDatabase = DatabasesOlapRepository.Search(filtro);
            ViewBag.Sottotitolo = TITOLO_LISTA_CUBI;
            if (id.HasValue)
                ViewBag.DatabaseId = id.Value;

            return PartialView("_CubiGestione", listaDatabase);
        }

        /// <summary>
        /// Restituisce la vista parziale contenente la lista dei cubi appartenenti al database specificato.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PartialViewCubiRicerca(int? id)
        {
            FiltroRicercaCubo filtro = new FiltroRicercaCubo { DatabaseId = id };
            CategoriaFiltro[] listaCubi = CubiRepository.Search(filtro);

            return PartialView("_CuboLista", listaCubi);
        }

        /// <summary>
        /// Abilita o disabilita l'utente specificato. 
        /// </summary>
        /// <param name="parametro">Identificatore dell'utente e valore dell'abilitazione. Esempio:
        ///     { PkUtenteId=1, Abilitato=true }
        /// </param>
        /// <returns></returns>
        [HttpPost]
        [WebMethod]
         public JsonResult AbilitaDisabilitaCubo(string _par)
        {
            EsitoPair<int> result = new EsitoPair<int>();
            try
            {
                CategoriaFiltro parametro = toCubo(_par);
                CategoriaFiltro cubo = CubiRepository.Load(parametro.PK_CategoriaFiltroId);
                cubo.Attivo = parametro.Attivo;
                CubiRepository.Update(cubo);
                result.EsitoOperazione.Esito = EsitoOperazioneType.OK;
            }
            catch (UserException ex)
            {
                result.EsitoOperazione.Messaggio = ExceptionUtility.GetMessageForAdministrator(ex);
            }
            catch (Exception ex)
            {
                GestisciEccezione(ex);
                result.EsitoOperazione.Messaggio = ExceptionUtility.GetMessageForAdministrator(ex);
            }

            return Json(result);
        }

        /// <summary>
        /// Restituisce la vista parziale per la creazione di un nuovo cubo.
        /// </summary>
        /// <param name="id">Identificatore del database di appartenenza del cubo</param>
        /// <returns></returns>
        public ActionResult PartialViewNuovoCubo(int id)
        {
            CategoriaFiltro cubo = creaCuboVuotoPerInserimento(id);
            ViewBag.Sottotitolo = TITOLO_NUOVO_CUBO;

            return PartialView("_CuboDettaglio", cubo);
        }

        //@@Gestire eccezioni
        /// <summary>
        /// Restituisce la vista parziale per la modifica del Cubo specificato.
        /// <summary>
        /// <param name="id">Identificatore del cubo da modificare</param>
        /// <returns></returns>
        public ActionResult PartialViewModificaCubo(int id)
        {
            CategoriaFiltro cubo;
            cubo = CubiRepository.Load(id);
            ViewBag.Sottotitolo = TITOLO_MODIFICA_CUBO;

            return PartialView("_CuboDettaglio", cubo);
        }

        /// <summary>
        /// Permette di inserire o modificare il cubo specificato
        /// </summary>
        /// <param name="_par">Cubo in formato json</param>
        /// <returns>Esito dell'operazione in formato json</returns>
        [HttpPost]
        public ActionResult InserisciModificaCubo(string _par)
        {
            EsitoPair<int> result = new EsitoPair<int>();
            try
            {
                CategoriaFiltro cubo = toCubo(_par);
                CubiRepository.InsertOrUpdate(cubo);
                result.EsitoOperazione.Esito = EsitoOperazioneType.OK;
            }
            catch (Exception ex)
            {
                GestisciEccezione(ex);
                result.EsitoOperazione.Messaggio = ExceptionUtility.GetMessageForAdministrator(ex);
            }

            return Json(result);
        }

        /// <summary>
        /// Restituisce la vista parziale per la gestione degli utenti (inserimeno/modifica/disabilitazione)
        /// </summary>
        /// <param name="username">Parametro opzionale, se specificato verranno restituiti gli
        /// utenti con la username contenente il parametro specificato</param>
        /// <returns></returns>
        //@@Gestire eccezioni
        public ActionResult PartialViewGestioneUtenti(string username)
        {
            FiltroRicercaUtente filtro = new FiltroRicercaUtente();
            filtro.Username = username;
            Utente[] listaUtenti = UtentiRepository.Search(filtro);
            ViewBag.Sottotitolo = TITOLO_LISTA_UTENTI;
            ViewBag.UserId = Sessione.Dati.UserId;

            return PartialView("_UtenteLista", listaUtenti);
        }

        /// <summary>
        /// Restituisce la vista parziale per la creazione o la modifica di un nuovo Utente.
        /// </summary>
        /// <param name="id">Identificatore dell'Utente (solo per ottenere la modifica)</param>
        /// <returns></returns>
        public ActionResult PartialViewInserisciModificaUtente(int? id)
        {
            Utente utente = creaUtenteVuotoPerInserimento();
            if (id.HasValue)
            {
                utente = UtentiRepository.Load(id.Value);
                ViewBag.Sottotitolo = TITOLO_MODIFICA_UTENTE;
            }
            else
            {
                utente = creaUtenteVuotoPerInserimento();
                ViewBag.Sottotitolo = TITOLO_NUOVO_UTENTE;
            }

            return PartialView("_UtenteDettaglio", utente);
        }

        [HttpPost]
        public ActionResult InserisciModificaUtente(string _par)
        {
            EsitoPair<Utente> result = new EsitoPair<Utente>();
            //EsitoOperazione result = new EsitoOperazione(EsitoOperazioneType.ERROR);
            try
            {
                Utente utente = toUtente(_par);
                result.Dati = UtentiRepository.InsertOrUpdate(utente);
                result.EsitoOperazione.Esito = EsitoOperazioneType.OK;
            }
            catch (Exception ex)
            {
                GestisciEccezione(ex);
                result.EsitoOperazione.Messaggio = ExceptionUtility.GetMessageForAdministrator(ex);
            }

            return Json(result);
        }

        /// <summary>
        /// Abilita o disabilita l'utente specificato. 
        /// </summary>
        /// <param name="parametro">Utente in formato json. Esempio:
        ///     { PkUtenteId=1, Abilitato=true }
        /// </param>
        /// <returns></returns>
        [HttpPost]
        [WebMethod]
        public JsonResult AbilitaDisabilitaUtente(string _par)
        {
            EsitoPair<int> result = new EsitoPair<int>();
            try
            {
                Utente parametro = toUtente(_par);
                Utente utente = UtentiRepository.Load(parametro.PkUtenteId);
                utente.Abilitato = parametro.Abilitato;
                UtentiRepository.Update(utente);

                result.EsitoOperazione.Esito = EsitoOperazioneType.OK;
            }
            catch (Exception ex)
            {
                GestisciEccezione(ex);
                result.EsitoOperazione.Messaggio = ExceptionUtility.GetMessageForAdministrator(ex);
            }

            return Json(result);
        }

        /// <summary>
        /// Resituisce la vista parziale per la gestione dei database (inserimento/modifica/disabilitazione database).
        /// </summary>
        /// <param name="nome">Parametro di ricerca opzionale per ottenere solo database contenenti il nome specificato</param>
        /// <returns></returns>
        /// <remarks>@@Gestire eccezioni</remarks>
        public ActionResult PartialViewElencoDatabase(string nome)
        {
            FiltroRicercaDatabase filtro = new FiltroRicercaDatabase();
            filtro.Nome = nome;
            DatabaseOlap[] listaDatabase = DatabasesOlapRepository.Search(filtro);
            ViewBag.Sottotitolo = TITOLO_LISTA_DATABASE;

            return PartialView("_DatabaseLista", listaDatabase);
        }

        /// <summary>
        /// Restituisce la vista parziale per l'inserimento o la modifica di un Database.
        /// </summary>
        /// <param name="id">In caso di modifica specificare l'identificatore del database da modificare</param>
        /// <returns></returns>
        public ActionResult PartialViewInserisciModificaDatabase(int? id)
        {
            PartialViewInserisciModificaDatabaseModel model = new PartialViewInserisciModificaDatabaseModel();

            FiltroRicercaServer filtroServer = new FiltroRicercaServer();
            filtroServer.Attivo = true;
            model.ListaServer = ServerOlapRepository.Search(filtroServer);

            if (id.HasValue)
            {
                model.Database = DatabasesOlapRepository.Load(id.Value);
                ViewBag.Sottotitolo = TITOLO_MODIFICA_DATABASE;
            }
            else
            {
                model.Database = creaDatabaseOlapVuotoPerInserimento();
                ViewBag.Sottotitolo = TITOLO_NUOVO_DATABASE;
            }

            return PartialView("_DatabaseDettaglio", model);
        }

        /// <summary>
        /// Inserisce o modifica il database Olap specificato.
        /// </summary>
        /// <remarks>
        /// @@Precisare il messaggio di errore quando l'utente chiede ad esempio:
        /// modifica
        ///     db01 in db02 e db02 già esiste. Attualmente viene restituito un generico messaggio di errore.
        /// </remarks>
        /// <param name="_par">stringa json per la descrizione dell'oggetto Database</param>
        /// <returns>Esito dell'operazione</returns>
        [HttpPost]
        public ActionResult InserisciModificaDatabase(string _par)
        {
            EsitoPair<int> result = new EsitoPair<int>();
            try
            {
                DatabaseOlap database = toDatabaseOlap(_par);
                DatabasesOlapRepository.InsertOrUpdate(database);
                result.EsitoOperazione.Esito = EsitoOperazioneType.OK;
            }
            catch (Exception ex)
            {
                GestisciEccezione(ex);
                result.EsitoOperazione.Messaggio = ExceptionUtility.GetMessageForAdministrator(ex);
            }

            return Json(result);
        }

        /// <summary>
        /// Abilita o disabilita il database specificato.
        /// </summary>
        /// <param name="_par">Stringa json contenente la modifica da apportare al database
        /// esempio { Id=1, Attivo=false } </param>
        /// <returns>Esito dell'operazione</returns>
        [HttpPost]
        [WebMethod]
        public JsonResult AbilitaDisabilitaDatabase(string _par)
        {
            EsitoPair<int> result = new EsitoPair<int>();
            try
            {
                DatabaseOlap parametro = toDatabaseOlap(_par);
                DatabaseOlap database = DatabasesOlapRepository.Load(parametro.Id);
                database.Attivo = parametro.Attivo;
                DatabasesOlapRepository.Update(database);

                result.EsitoOperazione.Esito = EsitoOperazioneType.OK;
            }
            catch (UserException ex)
            {
                result.EsitoOperazione.Messaggio = ExceptionUtility.GetMessageForAdministrator(ex);
            }
            catch (Exception ex)
            {
                GestisciEccezione(ex);
                result.EsitoOperazione.Messaggio = ExceptionUtility.GetMessageForAdministrator(ex);
            }

            return Json(result);
        }

        /// <summary>
        /// Crea un oggetto ServerOlap per consentire un nuovo inserimento.
        /// </summary>
        /// <returns></returns>
        private ServerOlap creaServerVuotoPerInserimento()
        {
            ServerOlap result = new ServerOlap
            {
                Attivo = true,
                Nome = "",
                Url = ""
            };

            return result;
        }

        /// <summary>
        /// Crea un oggetto Database per consentire un nuovo inserimento.
        /// </summary>
        /// <returns></returns>
        private DatabaseOlap creaDatabaseOlapVuotoPerInserimento()
        {
            DatabaseOlap result = new DatabaseOlap
            {
                Attivo = true,
                Nome = ""
            };

            return result;
        }

        /// <summary>
        /// Crea un oggetto Cubo per consentire un nuovo inserimento.
        /// </summary>
        /// <param name="databaseId"></param>
        /// <returns></returns>
        private CategoriaFiltro creaCuboVuotoPerInserimento(int databaseId)
        {
            CategoriaFiltro result = new CategoriaFiltro
            {
                DatabaseOlapId = databaseId,
                Attivo = true,
                CubeName = "",
                NomeCategoria = ""
            };

            return result;
        }

        /// <summary>
        /// Crea un oggetto Utente per consentire un nuovo inserimento.
        /// </summary>
        /// <returns></returns>
        private Utente creaUtenteVuotoPerInserimento()
        {
            Utente result = new Utente
                {
                    Abilitato = true,
                    Username = "",
                    RuoloId = 2
                };

            return result;
        }

        private List<MyCategoriaFiltro> creaVociMenu()
        {
            List<MyCategoriaFiltro> result = new List<MyCategoriaFiltro>();
            result.Add(new MyCategoriaFiltro(0, "Gestione Database"));
            result.Add(new MyCategoriaFiltro(0, "Gestione Cubi"));
            result.Add(new MyCategoriaFiltro(0, "Gestione Utenti"));

            return result;
        }

        /// <summary>
        /// Converte una stringa json in un oggetto Cubo
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        private ServerOlap toServerOlap(string jsonString)
        {
            dynamic par = JsonConvert.DeserializeObject(jsonString);
            ServerOlap result = new ServerOlap();
            if (par.Id != null)
                result.Id = par.Id;
            if (par.Nome != null)
                result.Nome = par.Nome;
            if (par.Url != null)
                result.Url = par.Url;
            if (par.Attivo != null)
                result.Attivo = par.Attivo;

            return result;
        }

        /// <summary>
        /// Converte una stringa json in oggetto Database
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        private DatabaseOlap toDatabaseOlap(string jsonString)
        {
            dynamic par = JsonConvert.DeserializeObject(jsonString);
            DatabaseOlap result = new DatabaseOlap();
            if (par.Id != null)
                result.Id = par.Id;
            if (par.Nome != null)
                result.Nome = par.Nome;
            if (par.ServerOlapId != null)
                result.ServerOlapId = par.ServerOlapId;
            if (par.Attivo != null)
                result.Attivo = par.Attivo;

            return result;
        }

        /// <summary>
        /// Converte una stringa json in un oggetto Cubo
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        private CategoriaFiltro toCubo(string jsonString)
        {
            dynamic par = JsonConvert.DeserializeObject(jsonString);
            CategoriaFiltro result = new CategoriaFiltro();
            if (par.Id != null)
                result.PK_CategoriaFiltroId = par.Id;
            if (par.DatabaseId != null)
                result.DatabaseOlapId = par.DatabaseId;
            if (par.Nome != null)
                result.CubeName = par.Nome;
            if (par.NomeFriendly != null)
                result.NomeCategoria = par.NomeFriendly;
            if (par.Attivo != null)
                result.Attivo = par.Attivo;

            return result;
        }

        /// <summary>
        /// Converte una stringa Json in un oggetto Utente
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        private Utente toUtente(string jsonString)
        {
            //Istruzione necessaria se la username è del tipo: dominio\username
            //Senza questa istruzione JsonConvert.DeserializeObject restituisce errore: Bad JSON escape sequence
            jsonString = jsonString.Replace("\\", "\\\\");

            dynamic par = JsonConvert.DeserializeObject(jsonString);
            Utente result = new Utente();
            result.PkUtenteId = par.PkUtenteId;
            result.Abilitato = par.Abilitato;
            if (par.RuoloId != null)
                result.RuoloId = par.RuoloId;
            if (par.Username != null)
                result.Username = par.Username;

            return result;
        }

        private int getUtenteId(string jsonString)
        {
            dynamic par = JsonConvert.DeserializeObject(jsonString);
            return par.PkUtenteId;
        }

        private int[] getCubiAutorizzati(string jsonString)
        {
            dynamic par = JsonConvert.DeserializeObject(jsonString);
            int[] result = par.arrayIntCubiAutorizzati.ToObject<int[]>();

            return result;
        }
        private static ServerOlap[] removeDatabaseNonAttiviEServerVuoti(ServerOlap[] listaServerOlap)
        {
            List<ServerOlap> result = new List<ServerOlap>();
            foreach (ServerOlap server in listaServerOlap)
            {
                server.DatabasesOlap = getDatabaseAttivi(server.DatabasesOlap);
                if (server.DatabasesOlap.Count > 0)
                    result.Add(server);
            }

            return result.ToArray();
        }

        private static DatabaseOlap[] getDatabaseAttivi(ICollection<DatabaseOlap> listaDatabase)
        {
            List<DatabaseOlap> result = new List<DatabaseOlap>();
            foreach (DatabaseOlap database in listaDatabase)
            {
                if (database.Attivo)
                    result.Add(database);
            }

            return result.ToArray();
        }


        const string TITOLO_LISTA_UTENTI = "Lista Utenti";
        const string TITOLO_NUOVO_UTENTE = "Nuovo Utente";
        const string TITOLO_MODIFICA_UTENTE = "Modifica Utente";

        const string TITOLO_LISTA_SERVER = "Lista Server";
        const string TITOLO_MODIFICA_SERVER = "Modifica Server";
        const string TITOLO_NUOVO_SERVER = "Nuovo Server";

        const string TITOLO_LISTA_DATABASE = "Lista Database";
        const string TITOLO_MODIFICA_DATABASE = "Modifica Database";
        const string TITOLO_NUOVO_DATABASE = "Nuovo Database";

        const string TITOLO_LISTA_CUBI = "Gestione Cubi";
        const string TITOLO_NUOVO_CUBO = "Nuovo Cubo";
        const string TITOLO_MODIFICA_CUBO = "Modifica Cubo";

        const string TITOLO_AUTORIZZAZIONI = "Autorizzazioni";
    }
}