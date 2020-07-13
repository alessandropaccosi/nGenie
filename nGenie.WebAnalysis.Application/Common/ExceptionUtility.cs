using nGenie.WebAnalysis.Application.Models;
using nGenie.WebAnalysis.Application.Models.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nGenie.WebAnalysis.Application.Common
{
    public class ExceptionUtility
    {
        /// <summary>
        /// Restituisce il messaggio da mostrare all'utente in base all'eccezione specificata
        /// </summary>
        /// <returns></returns>
        public static string GetFriendlyMessage(Exception ex)
        {
            return ex is UserException || ex is UtenteNonAutorizzatoException ?
                ex.Message
                : Costanti.MSG_ERRORE_GENERICO;
        }

        /// <summary>
        /// Restituisce il messaggio di errore diretto alle pagine di amministrazione.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetMessageForAdministrator(Exception ex)
        {
            //Per gli amministratori viene mostrato il dettaglio dell'eccezione. Eventualmente modificare.
            return string.Format("Errore durante l'operazione. Dettagli: {0}",
                ex.Message);
        }

        public static string GetMessage(Exception ex)
        {
            return getMessageChainAsString(ex, " ");
        }

        public static string GetStackTrace(Exception ex)
        {
            return getStackTraceChainAsString(ex);
        }

        private static string getMessageChainAsString(Exception ex, string separatore)
        {
            List<string> result = new List<string>();
            Exception currentException = ex;
            while (currentException != null)
            {
                result.Add(currentException.Message);
                currentException = currentException.InnerException;
            }

            return StringUtility.ToString(separatore, result);
        }

        private static string getStackTraceChainAsString(Exception ex)
        {
            List<string> result = new List<string>();
            Exception currentException = ex;
            while (currentException != null)
            {
                result.Add(currentException.StackTrace);
                currentException = currentException.InnerException;
            }

            return StringUtility.ToString("\r\n\r\n---Inner Stack", result);
        }
    }
}