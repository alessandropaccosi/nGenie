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
    public class UtentiRepository: BaseRepository, IDisposable
    {
        public UtentiRepository(): base()
        {
        }

        public static Utente InsertOrUpdate(Utente utente)
        {
            using (var repository = new UtentiRepository())
            {
                return repository.insertOrUpdate(utente);
            }
        }

        public static Utente Update(Utente utente)
        {
            using (var repository = new UtentiRepository())
            {
                return repository.update(utente);
            }
        }

        /// <summary>
        /// @@@###Permette di stabilire se l'Utente specificato è amministratore
        /// </summary>
        /// <returns>true se l'utente è Amministratore</returns>
        public static bool IsAdministrator(string username)
        {
            Utente utente = Load(username);
            return utente.RuoloId == (int)RuoloType.Amministratore;
        }

        /// <summary>
        /// Restituisce gli utenti con ruolo User abilitati al cubo specificato.
        /// </summary>
        /// <param name="utenteId">Utente che vuole inviare il report</param> 
        /// <param name="cuboId">Il cubo a cui il Report appartiene</param>
        /// <returns></returns>
        public static Utente[] SearchUtentiInvioReport(int utenteId, int cuboId)
        {
            using (var repository = new UtentiRepository())
            {
                return repository.searchUtentiInvioReport(utenteId, cuboId);
            }
        }

        public static Utente[] Search(FiltroRicercaUtente filtro)
        {
            using (var repository = new UtentiRepository())
            {
                return repository.search(filtro);
            }
        }

        public static Utente Load(int utenteId)
        {
            FiltroRicercaUtente filtro = new FiltroRicercaUtente();
            filtro.UtenteId = utenteId;
            Utente utente = Search(filtro).FirstOrDefault();
            if (utente == null)
                throw new UserException("Utente non esistente");

            return utente;
        }

        /// <summary>
        /// @@@###Restituisce l'Utente corrispondente alla username specificata
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static Utente Load(string username)
        {
            Utente utente;
            if (TryLoad(username, out utente))
            {
                return utente;
            }
            else
                throw new UserException("Username non esistente");
        }

        public static bool TryLoad(string username, out Utente utente)
        {
            FiltroRicercaUtente filtro = new FiltroRicercaUtente { Username = username };
            utente = Search(filtro).FirstOrDefault();

            return utente != null;
        }

        private Utente insertOrUpdate(Utente utente)
        {
            return utente.PkUtenteId <= 0 ? 
                insert(utente) 
                : update(utente);
        }

        private Utente insert(Utente utente)
        {
            if (usernameExists(utente.Username))
                throw new UserException("Username già esistente");

            Context.Utente.Add(utente);
            Context.SaveChanges();

            return utente;
        }

        private Utente update(Utente utente)
        {
            Context.Entry(utente).State = EntityState.Modified;
            Context.SaveChanges();

            return utente;
        }

        private bool usernameExists(string username)
        {
            FiltroRicercaUtente filtro = new FiltroRicercaUtente();
            filtro.Username = username;

            return search(filtro).Length > 0;
        }

        private Utente[] search(FiltroRicercaUtente filtro)
        {
            var query = 
                    from
                        utente in Context.Utente.Include("Ruoli")
                    where
                        (string.IsNullOrEmpty(filtro.Username) || utente.Username.ToLower() == filtro.Username.ToLower())
                            &&
                        (!filtro.UtenteId.HasValue || utente.PkUtenteId == filtro.UtenteId)
                    select
                        utente;

            return query.ToArray();
        }

        private Utente[] searchUtentiInvioReport(int utenteId, int cuboId)
        {
            Context.Configuration.ProxyCreationEnabled = false;
            var query =
                    from
                        Autorizzazioni in Context.AutorizzazioniCubi
                            join Utenti in Context.Utente //.Include("Ruoli")
                                on Autorizzazioni.UtenteId equals Utenti.PkUtenteId
                            join Ruoli in Context.Ruoli
                                on Utenti.RuoloId equals Ruoli.Id
                    where
                        Autorizzazioni.CuboId == cuboId
                            &&
                        Utenti.Abilitato
                            &&
                        Ruoli.Id == (int)RuoloType.Utente
                            &&
                        Utenti.PkUtenteId != utenteId
                    select
                        Utenti;

            return query.ToArray();
        }

        
    }
}