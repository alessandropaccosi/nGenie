using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using nGenie.WebAnalysis.Application.DataAccess.Repository;
using nGenie.WebAnalysis.Application.Models;
using nGenie.WebAnalysis.Application.Models.DbModel;

namespace nGenie.WebAnalysis.Application.Common
{
    /// <summary>
    /// Permette di accedere ai dati di sessione in modo strutturato.
    /// In caso la sessione non esista tenta di ricrearla.
    /// </summary>
    public class Sessione
    {

        /// <summary>
        /// Restituisce true se la sessione e' presente
        /// </summary>
        public static bool IsValid
        {
            get
            {
                return 
                    System.Web.HttpContext.Current.Session != null
                        &&
                    System.Web.HttpContext.Current.Session[Costanti.SESSION_KEY] != null;
            }
        }

        // Elimina i dati della sessione
        public static void Abandon()
        {
            if (HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session.Clear();
                HttpContext.Current.Session.Abandon();
            }
        }

        /// <summary>
        /// Permette di accedere in modo strutturato ai dati memorizzati nella Sessione.
        /// </summary>
        public static DatiSessione Dati
        {
            get
            {
                DatiSessione result = new DatiSessione();
                System.Web.SessionState.HttpSessionState sessioneCorrente = System.Web.HttpContext.Current.Session;
                if (!Sessione.IsValid)
                {
                    DatiSessione datiSessione = createInstanceDatiSessione();

                    if (sessioneCorrente != null)
                    {
                        //Ricrea la sessione
                        sessioneCorrente[Costanti.SESSION_KEY] = datiSessione;
                    }
                    else
                    {
                        //Non puo' creare la sessione ma possiede i dati di sessione e li restituisce
                        result = datiSessione;
                    }
                }
                else
                {
                    result = (DatiSessione) sessioneCorrente[Costanti.SESSION_KEY];
                }

                return result;
            }
        }

        /// <summary>
        /// Imposta i dati in Sessione nel caso non siano presenti.
        /// Verranno utilizzati i dati dell'utente attualmente autorizzato.
        /// </summary>
        public static void ImpostaSessioneIfNotExistsAndUserIsValid()
        {
            if (!IsValid && IdentityUtility.IsValid)
            {
                Utente utente = getUtenteAutorizzato();

                //Richiede la creazione della sessione in modo da riportare informazioni sull'utente
                impostaSessione(utente);
            }
        }

        /// <summary>
        /// Crea la Sessione per la username specificata
        /// </summary>
        /// <param name="username"></param>
        public static void ImpostaSessione(string username)
        {
            //Carica dal database informazioni sull'utente specificato.
            Utente utente;
            if (UtentiRepository.TryLoad(username, out utente))
            {
                //Richiede la creazione della sessione in modo da riportare informazioni sull'utente
                impostaSessione(utente);
            }
            else
                throw new UtenteNonAutorizzatoException();
        }

        /// <summary>
        /// Crea la sessione per l'utente specificato
        /// </summary>
        /// <param name="utente">L'utente deve contenere anche informazioni sul Ruolo</param>
        private static void impostaSessione(Utente utente)
        {
            DatiSessione datiSessione = createInstanceDatiSessione(utente);
            System.Web.HttpContext.Current.Session[Costanti.SESSION_KEY] = datiSessione;
        }

        /// <summary>
        /// Crea i dati che dovranno essere messi in sessione
        /// </summary>
        /// <returns></returns>
        private static DatiSessione createInstanceDatiSessione()
        {
            Utente utente = getUtenteAutorizzato();
            return createInstanceDatiSessione(utente);
        }

        /// <summary>
        /// Restituisce i dati che dovranno essere messi in sessione per l'utente specificato
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        private static DatiSessione createInstanceDatiSessione(Utente utente)
        {
            DatiSessione result = new DatiSessione();
            result.UserId = utente.PkUtenteId;
            result.Username = utente.Username;
            result.RuoloId = utente.RuoloId;
            result.IsAdministrator = utente.Ruoli.IsAdministrator;
            result.Cognome = utente.Cognome;
            result.Nome = utente.Nome;

            return result;
        }

        /// <summary>
        /// Restituisce l'utente attualmente autorizzato.
        /// </summary>
        /// <returns></returns>
        private static Utente getUtenteAutorizzato()
        {
            string username = getUsernameUtenteAutorizzato();

            //Carica dal database informazioni sull'utente specificato.
            Utente utente;
            if (UtentiRepository.TryLoad(username, out utente))
            {
                return utente;
            }
            else
                throw new UtenteNonAutorizzatoException();
        }

        /// <summary>
        /// Restiuisce la username dell'utente attualmente autenticato
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UserException">Nel caso l'Utente non risulta autenticato oppure username null o stringa vuota</exception>
        private static string getUsernameUtenteAutorizzato()
        {
            string result = IdentityUtility.UsernameWithoutDomain;

            if (!IdentityUtility.UserIsAuthenticated)
                throw new UtenteNonAutorizzatoException("L'utente non risulta autenticato. Riconnettersi al sito.");

            if (string.IsNullOrEmpty(result))
                throw new UtenteNonAutorizzatoException("Identità dell'utente non valorizzata. Provare a riconnettersi al sito.");

            return result;
        }
    }
}