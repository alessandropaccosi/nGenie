using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using nGenie.WebAnalysis.Application.Models.DbModel;

namespace nGenie.WebAnalysis.Application.Models
{
    public class UtenteAutorizzazioniCubi
    {
        public Utente Utente = new Utente();
        public ServerOlap[] ListaServer = new ServerOlap[] { };
        public DatabaseOlap[] ListaDatabase = new DatabaseOlap[] { };
        public CategoriaFiltro[] ListaCubi = new CategoriaFiltro[] { };
        public AutorizzazioneCubo[] ListaAutorizzazioniCubi = new AutorizzazioneCubo[]{};
    }
}