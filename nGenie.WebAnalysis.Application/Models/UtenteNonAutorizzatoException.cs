using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nGenie.WebAnalysis.Application.Models
{
    public class UtenteNonAutorizzatoException : Exception
    {
        public UtenteNonAutorizzatoException()
            : base("Accesso non autorizzato")
        {

        }

        public UtenteNonAutorizzatoException(string message)
            : base(message)
        {

        }
    }
}