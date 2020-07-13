using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nGenie.WebAnalysis.Application.Models
{
    public class ConnessioneHubDeleted
    {
        public string ConnectionId = "";
        public DateTime DataEliminazione;
        public bool MarcatoDaEliminare = false;
    }
}