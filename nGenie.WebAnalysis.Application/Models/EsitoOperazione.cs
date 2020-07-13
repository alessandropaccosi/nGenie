using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nGenie.WebAnalysis.Application.Models
{
    [Serializable]
    public class EsitoOperazione
    {
        public EsitoOperazione(EsitoOperazioneType Esito)
        {
            this.Esito = Esito;
        }

        public EsitoOperazione(string messaggio, EsitoOperazioneType esito)
        {
            Messaggio = messaggio;
            Esito = esito;
        }

        public int Codice;
        public string Messaggio = "";
        public EsitoOperazioneType Esito = EsitoOperazioneType.OK;
    }
}