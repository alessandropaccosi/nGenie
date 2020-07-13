using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nGenie.WebAnalysis.Application.Models
{
    /// <summary>
    /// Contiene dati della Sessione
    /// </summary>
    [Serializable]
    public class DatiSessione
    {
        public int UserId;
        public string Username = "";
        public int RuoloId;
        public bool IsAdministrator;
        public string Cognome = "";
        public string Nome = "";

        public string GetNomeEsteso()
        {
            return string.Format("{0} {1}", Nome, Cognome);
        }
    }
}