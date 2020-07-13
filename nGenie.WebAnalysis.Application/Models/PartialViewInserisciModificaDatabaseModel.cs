using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using nGenie.WebAnalysis.Application.Models.DbModel;

namespace nGenie.WebAnalysis.Application.Models
{
    //Contiene i dati da restituire alla vista parziale per la modifica/inserimento di un database
    public class PartialViewInserisciModificaDatabaseModel
    {
        public DatabaseOlap Database = new DatabaseOlap();
        public ServerOlap[] ListaServer = new ServerOlap[] { };
    }
}