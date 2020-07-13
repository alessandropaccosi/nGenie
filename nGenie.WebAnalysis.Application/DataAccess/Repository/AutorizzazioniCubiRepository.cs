using Newtonsoft.Json;
using nGenie.WebAnalysis.Application.DataAccess.Interfaces;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using nGenie.WebAnalysis.Application.Models;
using nGenie.WebAnalysis.Application.Models.DbModel;

namespace nGenie.WebAnalysis.Application.DataAccess.Repository
{
    public class AutorizzazioniCubiRepository : BaseRepository, IDisposable
    {
        public AutorizzazioniCubiRepository() : base()
        {
        }

        //@@Temporanea, da sistemare
        public static void ImpostaAutorizzazioni(int utenteId, int[] cubiAutorizzati)
        {
            using (var repository = new AutorizzazioniCubiRepository())
            {
                repository.delete(utenteId);
                foreach (int cuboId in cubiAutorizzati)
                {
                    AutorizzazioneCubo autorizzazione = new AutorizzazioneCubo();
                    autorizzazione.UtenteId = utenteId;
                    autorizzazione.CuboId = cuboId;

                    repository.insert(autorizzazione);
                }
            }
        }

        /// <summary>
        /// Elimina tutte le autorizzazioni dell'Utente specificato.
        /// </summary>
        /// <param name="utenteId"></param>
        public static void Delete(int utenteId)
        {
            using (var repository = new AutorizzazioniCubiRepository())
            {
                repository.delete(utenteId);
            }
        }

        /// <summary>
        /// Restituisce true se e' presente un autorizzazione sul Cubo per l'utente specificato, il Cubo e' attivo
        /// e riferisce un Database e un Server attivi.
        /// Non riportare un utente Amministratore in quanto gli amministratori non hanno esplicite autorizzazioni sui Cubi.
        /// <param name="utenteId"></param>
        /// <param name="cuboId"></param>
        public static bool IsUtenteAutorizzatoCubo(int utenteId, int cuboId)
        {
            FiltroRicercaAutorizzazioniCubo filtro = new FiltroRicercaAutorizzazioniCubo();
            filtro.CuboId = cuboId;
            filtro.UtenteId = utenteId;
            AutorizzazioneCubo[] autorizzazioni = Search(filtro);

            return Search(filtro).Length > 0;
        }

        public static AutorizzazioneCubo[] Search(FiltroRicercaAutorizzazioniCubo filtro)
        {
            using (var repository = new AutorizzazioniCubiRepository())
            {
                return repository.search(filtro);
            }
        }

        /// <summary>
        /// Restituisce i Cubi a cui l'utente è autorizzato. Vengono restituiti solo i Cubi attivi che riferiscono
        /// Database e Server attivi.
        /// </summary>
        /// <param name="utenteId"></param>
        /// <returns></returns>
        public static AutorizzazioneCubo[] Load(int utenteId)
        {
            FiltroRicercaAutorizzazioniCubo filtro = new FiltroRicercaAutorizzazioniCubo { UtenteId = utenteId };
            return Search(filtro);
        }

        //public static void InsertReportPredefinitoIfNotExists(int utenteId, int cuboId)
        //{
        //    using (var repository = new AutorizzazioniCubiRepository())
        //    {
        //        AutorizzazioneCubo autorizzazione = new AutorizzazioneCubo();
        //        autorizzazione.UtenteId = utenteId;
        //        autorizzazione.CuboId = cuboId;
        //        repository.insertReportPredefinitoIfNotExists(autorizzazione);
        //    }
        //}

        // 
        // Metodi privati
        //
        private int insert(AutorizzazioneCubo autorizzazione)
        {
            Context.AutorizzazioniCubi.Add(autorizzazione);
            Context.SaveChanges();

            //insertReportPredefinitoIfNotExists(autorizzazione);


            return autorizzazione.Id;
        }

        //private void insertReportPredefinitoIfNotExists(AutorizzazioneCubo autorizzazione)
        //{
        //    FiltroRicercaReport filtro = new FiltroRicercaReport();
        //    filtro.UtenteId = autorizzazione.UtenteId;
        //    filtro.CuboId = autorizzazione.CuboId;
        //    filtro.Predefinito = true;

        //    if (!ReportRepository.Exists(filtro))
        //    {
        //        FiltroUtente report = new FiltroUtente();
        //        report.Nome = "Report base";
        //        report.DataCreazione = DateTime.Now;
        //        report.DataUltimaModifica = DateTime.Now;
        //        report.FK_UtenteId = autorizzazione.UtenteId;
        //        report.FK_CategoriaId = autorizzazione.CuboId;
        //        report.Colonne = "[]";
        //        report.Righe = "[]";
        //        report.Misure = "[]";
        //        report.Transport = getTransport(autorizzazione);
        //        report.Filtri = "";
        //        report.Sort = "";
        //        report.Attivo = true;
        //        report.Predefinito = true;
        //        Context.FiltroUtente.Add(report);
        //        Context.SaveChanges();
        //    }
        //}

        private string getTransport(AutorizzazioneCubo autorizzazione)
        {
            CategoriaFiltro cubo = CubiRepository.Load(autorizzazione.CuboId);
            DatabaseOlap database = DatabasesOlapRepository.Load(cubo.DatabaseOlapId);
            
            string s =
                @"
{
  ""transport"": {
    ""options"": {
      ""read"": {
        ""dataType"": ""text"",
        ""contentType"": ""text/xml"",
        ""type"": ""POST"",
        ""url"": ""SERVER_OLAP_URL""
      },
      ""discover"": {
        ""dataType"": ""text"",
        ""contentType"": ""text/xml"",
        ""type"": ""POST"",
        ""url"": ""SERVER_OLAP_URL""
      },
      ""connection"": {
        ""catalog"": ""DATABASE_OLAP_NOME"",
        ""cube"": ""CUBO_NOME""
      }
    },
    ""cache"": {}
  },
  ""options"": {
    ""read"": {
      ""dataType"": ""text"",
      ""contentType"": ""text/xml"",
      ""type"": ""POST"",
      ""url"": ""SERVER_OLAP_URL""
    },
    ""discover"": {
      ""dataType"": ""text"",
      ""contentType"": ""text/xml"",
      ""type"": ""POST"",
      ""url"": ""SERVER_OLAP_URL""
    },
    ""connection"": {
      ""catalog"": ""DATABASE_OLAP_NOME"",
      ""cube"": ""CUBO_NOME""
    }
  }
}

                ";

            s = s.Replace("DATABASE_OLAP_NOME", database.Nome);
            s = s.Replace("CUBO_NOME", cubo.CubeName);
            s = s.Replace("SERVER_OLAP_URL", database.ServerOlap.Url);

            return s;
        }

        private AutorizzazioneCubo[] search(FiltroRicercaAutorizzazioniCubo filtro)
        {
            var query =
                    from
                        autorizzazione in Context.AutorizzazioniCubi
                            join Cubi in Context.CategoriaFiltro
                                on autorizzazione.CuboId equals Cubi.PK_CategoriaFiltroId
                            join Database in Context.DatabasesOlap
                                on Cubi.DatabaseOlapId equals Database.Id
                            join Server in Context.ServerOlap
                                on Database.ServerOlapId equals Server.Id
                    where
                        (!filtro.UtenteId.HasValue || autorizzazione.UtenteId == filtro.UtenteId)
                            &&
                        (!filtro.CuboId.HasValue || autorizzazione.CuboId == filtro.CuboId)
                            &&
                        Cubi.Attivo
                            &&
                        Database.Attivo
                            &&
                        Server.Attivo
                    select
                        autorizzazione;

            return query.ToArray();
        }
        private void delete(int utenteId)
        {
            var query =
                    from
                        autorizzazione in Context.AutorizzazioniCubi
                    where
                        autorizzazione.UtenteId == utenteId
                    select
                        autorizzazione;

            foreach (var item in query)
            {
                Context.AutorizzazioniCubi.Remove(item);
            }

            Context.SaveChanges();
        }
    }
}