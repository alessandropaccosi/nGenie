using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nGenie.WebAnalysis.Application.Common
{
    public class Costanti
    {
        public const string NOME_APPLICAZIONE = "nGenie Analysis";
        public const string MSG_ERRORE_GENERICO = "Si è verificato un errore durante l'operazione";
        public const string MSG_SESSIONE_SCADUTA = "Sessione o autorizzazione scaduta. E' necessario ricaricare la pagina";

        /// <summary>
        /// Chiave utilizzata ad esempio per passare ad una vista un messaggio di errore.
        /// </summary>
        public const string KEY_ERROR_MESSAGE = "ShowErrorMessage";

        /// <summary>
        /// Codice errore sessione scaduta
        /// </summary>
        public const int KEY_ERROR_SESSIONE_SCADUTA = 1;

        /// <summary>
        /// Chiave per accedere ai dati della sessione
        /// </summary>
        public const string SESSION_KEY = "sessione";

        public const string MSG_AUTORIZZAZIONE_SCADUTA = "Autorizzazione scaduta, è necessario riconnettersi al sito.";

        public const string MSG_NESSUN_CUBO_AUTORIZZATO = "L'utente non è stato abilitato su nessun Cubo, si consiglia di contattare il supporto tecnico in modo da richiedere l'abilitazione.";

        public const string MSG_NESSUN_CUBO_ATTIVO = "Non esiste nessun cubo attivo";

        public const string MSG_CUBO_NON_TROVATO = "Cubo non trovato. E' possibile che il cubo non sia più attivo";

        public const string MSG_CUBO_NON_AUTORIZZATO = "Cubo non autorizzato";

        public const string MSG_CUBO_NON_ESISTENTE = "Cubo non trovato";


    }
}