using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using nGenie.WebAnalysis.Application.Common;
using nGenie.WebAnalysis.Application.DataAccess.Repository;
using nGenie.WebAnalysis.Application.Models;
using nGenie.WebAnalysis.Application.Models.DbModel;

namespace nGenie.WebAnalysis.Application.Models
{
    public class AnalysisUtente
    {
        public string UltimoFiltro { get; set; }
        public List<MyCategoriaFiltro> CategorieFiltri = new List<MyCategoriaFiltro>();
        public DatabaseOlap[] ListaDatabase = new DatabaseOlap[] { };

        public AnalysisUtente()
        {
            CategorieFiltri = getCategorieFiltri();
        }

        public AnalysisUtente(List<MyCategoriaFiltro> listaCategorie)
        {
            CategorieFiltri = listaCategorie;
        }

        private List<MyCategoriaFiltro> getCategorieFiltri()
        {
            List<MyCategoriaFiltro> catFiltri = new List<MyCategoriaFiltro>();
            bool isAdministrator = Sessione.Dati.IsAdministrator;
            using (TempCaringServiceEntities ctx = new TempCaringServiceEntities())
            {
                CategoriaFiltro[] listaCubi = getCubiAnalysis(ctx); 

                List<DatabaseOlap> listaDatabase = new List<DatabaseOlap>();
                foreach (CategoriaFiltro cubo in listaCubi)
                {
                    //Costruisce la lista dei database riferiti dai cubi.
                    if (listaDatabase.Find(e => e.Id == cubo.DatabaseOlapId) == null)
                    {
                        listaDatabase.Add(DatabasesOlapRepository.Load(cubo.DatabaseOlapId));
                    }

                    IEnumerable<FiltroUtente> reportsUtente = cubo.FiltroUtente.Where(
                        report => report.Attivo && !report.Condiviso && (isAdministrator || report.FK_UtenteId == Sessione.Dati.UserId)
                     );
                    MyCategoriaFiltro MyCat = new MyCategoriaFiltro(cubo, reportsUtente,  Sessione.Dati.UserId);
                    catFiltri.Add(MyCat);
                }

                ListaDatabase = listaDatabase.ToArray();
            }

            return catFiltri;
        }

        private CategoriaFiltro[] getCubiAnalysis(TempCaringServiceEntities context)
        {
            bool isAdministrator = Sessione.Dati.IsAdministrator;
            IQueryable<CategoriaFiltro> query;

            if (!isAdministrator)
            {
                query =
                (
                    from
                        Autorizzazioni in context.AutorizzazioniCubi
                    join Cubi in context.CategoriaFiltro
                            on Autorizzazioni.CuboId equals Cubi.PK_CategoriaFiltroId
                    join Database in context.DatabasesOlap
                           on Cubi.DatabaseOlapId equals Database.Id
                    join Server in context.ServerOlap
                            on Database.ServerOlapId equals Server.Id
                    join Utenti in context.Utente
                            on Autorizzazioni.UtenteId equals Utenti.PkUtenteId
                    where
                        (Autorizzazioni.UtenteId == Sessione.Dati.UserId)
                            &&
                        (Utenti.Abilitato)
                            &&
                        (Cubi.Attivo)
                            &&
                        (Database.Attivo)
                            &&
                        (Server.Attivo)
                    orderby
                        Server.Nome ascending
                //from
                //    Report in ctx.FiltroUtente.Where(r=> r.FK_CategoriaId == Autorizzazioni.CuboId && r.FK_UtenteId == Autorizzazioni.UtenteId).DefaultIfEmpty()
                select
                Cubi
                 );
            }
            else
            {
                query =
                (
                    from    
                        Cubi in context.CategoriaFiltro
                    join Database in context.DatabasesOlap
                           on Cubi.DatabaseOlapId equals Database.Id
                    join Server in context.ServerOlap
                            on Database.ServerOlapId equals Server.Id
                    where
                        (Cubi.Attivo)
                            &&
                        (Database.Attivo)
                            &&
                        (Server.Attivo)
                    orderby
                        Server.Nome ascending
                    //from
                    //    Report in ctx.FiltroUtente.Where(r=> r.FK_CategoriaId == Autorizzazioni.CuboId && r.FK_UtenteId == Autorizzazioni.UtenteId).DefaultIfEmpty()
                    select
                    Cubi
                 );
            }

            return query.ToArray();
        }
    

//        private List<MyCategoriaFiltro> getCategorieFiltri()
//        {
//            //Eliminare
//            //getCategorieFiltri2();

//            List<MyCategoriaFiltro> catFiltri = new List<MyCategoriaFiltro>();
//            using (TempCaringServiceEntities ctx = new TempCaringServiceEntities())
//            {
//                bool isAdministrator = Sessione.Dati.IsAdministrator;

//                var dbCategorieFiltri =
//                (
//                    from
//                        Report in ctx.FiltroUtente
//                    join Cubi in ctx.CategoriaFiltro
//                        on Report.FK_CategoriaId equals Cubi.PK_CategoriaFiltroId
//                    join Database in ctx.DatabasesOlap
//                        on Cubi.DatabaseOlapId equals Database.Id
//                    join Server in ctx.ServerOlap
//                        on Database.ServerOlapId equals Server.Id
//                    join Utenti in ctx.Utente
//                        on Report.FK_UtenteId equals Utenti.PkUtenteId
//                    from
//                        Autorizzazioni in ctx.AutorizzazioniCubi

//                    where
//                            (isAdministrator || Report.FK_UtenteId == Sessione.Dati.UserId)
//                                &&
//                            (Utenti.Abilitato)
//                                &&
//                            (Report.Attivo)
//                                &&
//                            (Cubi.Attivo)
//                                &&
//                            (Database.Attivo)
//                                &&
//                            (Server.Attivo)
//                                &&
//                            (isAdministrator || Report.FK_UtenteId == Autorizzazioni.UtenteId)
//                                &&
//                            (isAdministrator || Autorizzazioni.CuboId == Cubi.PK_CategoriaFiltroId)
//                    orderby
//                        Cubi.NomeCategoria ascending
//                    select
//                        Cubi).Distinct().ToList();

//                List<DatabaseOlap> listaDatabase = new List<DatabaseOlap>();
//                foreach (CategoriaFiltro c in dbCategorieFiltri)
//                {
//                    //Costruisce la lista dei database riferiti dai cubi.
//                    if (listaDatabase.Find(e => e.Id == c.DatabaseOlapId) == null)
//                    {
//                        listaDatabase.Add(DatabasesOlapRepository.Load(c.DatabaseOlapId));
//                    }

//                    MyCategoriaFiltro MyCat = new MyCategoriaFiltro(c);
//                    catFiltri.Add(MyCat);
//                }

//                ListaDatabase = listaDatabase.ToArray();
//            }

//            return catFiltri;
//        }


}

public class MyCategoriaFiltro
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        public int DatabaseOlapId { get; set; }

        public List<MyFiltro> Filtri = new List<MyFiltro>();

        public MyCategoriaFiltro(int id, string nome)
        {
            this.Id = id;
            this.Nome = nome;
        }

        public MyCategoriaFiltro(CategoriaFiltro cubo, IEnumerable<FiltroUtente> reportsUtente, int utenteId)
        {
            this.Nome = cubo.NomeCategoria;
            this.Id = cubo.PK_CategoriaFiltroId;
            this.DatabaseOlapId = cubo.DatabaseOlapId;
            if (reportsUtente.Count() == 0)
            {
                FiltroUtente report = GetNuovoReport(cubo);
                MyFiltro myFiltro = new MyFiltro(report);
                Filtri.Add(myFiltro);
            }
            else
            //foreach (FiltroUtente f in cubo.FiltroUtente)
            foreach (FiltroUtente report in reportsUtente)
            {
                if (
                        (
                            report.FK_UtenteId == Sessione.Dati.UserId
                                || 
                            Sessione.Dati.IsAdministrator
                        ) 
                            && 
                        (report.Attivo == true)
                    )
                {
                    MyFiltro myFiltro = new MyFiltro(report);
                    Filtri.Add(myFiltro);
                }
            }
        }

        /// <summary>
        /// Restituisce un nuovo filtro per il Cubo specificato.
        /// </summary>
        /// <param name="cubo"></param>
        /// <exception cref="UserException">Sollevata se l'utente non possiede i diritti sul Cubo specificato</exception>
        /// <returns></returns>
        public static FiltroUtente GetNuovoReport(int cuboId)
        {
            //Ottiene informazioni sul Cubo
            FiltroRicercaCubo filtroRicercaCubo = new FiltroRicercaCubo();
            filtroRicercaCubo.CuboId = cuboId;

            //Riporta nel filtro di ricerca che siamo interessati solo ad un cubo attivo che riferisce database e server attivi
            filtroRicercaCubo.Attivo = true;
            CategoriaFiltro cubo = CubiRepository.Search(filtroRicercaCubo).FirstOrDefault();
            if (cubo == null)
                throw new UserException(Costanti.MSG_CUBO_NON_TROVATO);

            return GetNuovoReport(cubo);
        }

        /// <summary>
        /// Restituisce un nuovo filtro per il Cubo specificato.
        /// </summary>
        /// <param name="cubo"></param>
        /// <exception cref="UserException">Sollevata se l'utente non possiede i diritti sul Cubo specificato</exception>
        /// <returns></returns>
        public static FiltroUtente GetNuovoReport(CategoriaFiltro cubo)
        {
            FiltroUtente result = new FiltroUtente();

            //Se l'utente non e' amministratore controlla se possiede i diritti di accesso al cubo
            if (!Sessione.Dati.IsAdministrator)
            {
                if (!AutorizzazioniCubiRepository.IsUtenteAutorizzatoCubo(Sessione.Dati.UserId, cubo.PK_CategoriaFiltroId))
                    throw new UserException(Costanti.MSG_CUBO_NON_AUTORIZZATO);
            }

            //Viene assegnato come id l'identificativo del cubo e invertito di segno
            result.PKFiltroId = -cubo.PK_CategoriaFiltroId;

            result.Nome = "Nuovo Report";
            result.DataCreazione = DateTime.Now;
            result.DataUltimaModifica = DateTime.Now;
            result.FK_UtenteId =  Sessione.Dati.UserId;
            result.FK_CategoriaId = cubo.PK_CategoriaFiltroId;
            result.Colonne = "[]";
            result.Righe = "[]";
            result.Misure = "[]";
            result.Transport = "";// getTransport(autorizzazione);
            result.Filtri = "";
            result.Sort = "";
            result.Attivo = true;
            result.Predefinito = true;
            result.CategoriaFiltro = cubo;

            result.Utente = new Utente();
            result.Utente.Abilitato = true;

            return result;
        }
    }

    public class MyFiltro
    {
        public MyFiltro()
        {
        }

        public MyFiltro(FiltroUtente f)
        {
            Nome = f.Nome;
            Id = f.PKFiltroId;
            Colonne = f.Colonne;
            Righe = f.Righe;
            Misure = f.Misure;
            Transport = f.Transport;
            IdCategoria = f.FK_CategoriaId;
            IdUtente = f.FK_UtenteId;
            Filtri = f.Filtri;
            Sort = f.Sort;
            NomeCategoria = f.CategoriaFiltro.NomeCategoria;
            CubeName = f.CategoriaFiltro.CubeName;
            Predefinito = f.Predefinito;
            UtenteNome = f.Utente.Nome;
            UtenteCognome = f.Utente.Cognome;
            Condiviso = f.Condiviso;
            //Utente = f.Utente;

            DatabaseOlap database = DatabasesOlapRepository.Load(f.CategoriaFiltro.DatabaseOlapId);
            DbName = database.Nome;
            IdDatabase = database.Id;
            ServerOlapUrl = database.ServerOlap.Url;
        }

        public string Nome { get; set; }
        public string NomeCategoria { get; set; }
        [Key]
        public int Id { get; set; }
        public int IdCategoria { get; set; }
        public string Colonne { get; set; }
        public string Righe { get; set; }
        public string Misure { get; set; }
        public string Transport { get; set; }
        public int IdUtente { get; set; }
        public string Filtri { get; set; }
        public string Sort { get; set; }

        public string CubeName { get; set; }

        public string DbName { get; set; }
        public int IdDatabase { get; set; }
        public bool Predefinito { get; set; }
        public string ServerOlapUrl { get; set; }

        public string UtenteNome { get; set; }
        public string UtenteCognome { get; set; }
        public bool Condiviso { get; set; }

        //public Utente Utente { get; set; }
    }
}

