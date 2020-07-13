using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//@@Cambiare namespace
namespace nGenie.WebAnalysis.Application.Models
{
    /// <summary>
    /// Eccezzione contenente un messaggio di errore diretto all'utente finale
    /// </summary>
    public class UserException : Exception
    {
        public UserException(string message)
            : base(message)
        {

        }
    }
}