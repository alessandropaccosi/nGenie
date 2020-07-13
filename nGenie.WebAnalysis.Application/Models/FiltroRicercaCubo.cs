using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nGenie.WebAnalysis.Application.Models
{
    /// <summary>
    /// @@@###Filtro per la ricerca di Cubi
    /// </summary>
    public class FiltroRicercaCubo
    {
        public int? CuboId;

        public int? DatabaseId;

        public string Nome;

        /// <summary>
        /// Se impostato a true richiede di cercare solo Cubi attivi che riferiscono Database e Server attivi
        /// </summary>
        public bool? Attivo;

        //public int? UtenteId;
    }
}