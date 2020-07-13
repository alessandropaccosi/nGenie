using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using nGenie.WebAnalysis.Application.Models.DbModel;

namespace nGenie.WebAnalysis.Application.Models
{
    public class ReportRicevutoInfo
    {
        public Utente Mittente = new Utente();
        public Utente Destinatario = new Utente();
        public ReportCondiviso ReportCondiviso = new ReportCondiviso();
        public FiltroUtente Report = new FiltroUtente();
        public CategoriaFiltro Cubo = new CategoriaFiltro();
        public ServerOlap ServerOlap = new ServerOlap();
    }
}