using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nGenie.WebAnalysis.Application.Models
{
    /// <summary>
    /// Filtro per la ricerca di Database Olap
    /// </summary>
    public class FiltroRicercaDatabase
    {
        public int? DatabaseOlapId;

        public string Nome;

        public bool? Attivo;
    }
}