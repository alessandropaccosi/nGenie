using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nGenie.WebAnalysis.Application.Models
{
    public class EsitoPair<TResult>
    {
        public EsitoPair()
        {

        }

        public EsitoOperazione EsitoOperazione = new EsitoOperazione(EsitoOperazioneType.ERROR);
        public TResult Dati;
    }
}