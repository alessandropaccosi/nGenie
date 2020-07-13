using nGenie.WebAnalysis.Application.DataAccess.Repository;
using nGenie.WebAnalysis.Application.Models.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nGenie.WebAnalysis.Application.Common
{
    public class LogUtility
    {
        /// <summary>
        /// Inserisce l'eccezione nel log. Non solleva eccezioni.
        /// </summary>
        /// <param name="ex"></param>
        public static void InsertLogWithoutException(Exception ex)
        {
            try
            {
                Log log = new Log();
                log.Messaggio = ExceptionUtility.GetMessage(ex);
                log.StackTrace = ExceptionUtility.GetStackTrace(ex);

                LogRepository.Insert(log);
            }
            catch
            {
            }
        }

        public static void InsertTraceWithoutException(string message)
        {
            try
            {
                Log log = new Log();
                log.Messaggio = message;

                LogRepository.Insert(log);
            }
            catch
            {
            }
        }
    }
}