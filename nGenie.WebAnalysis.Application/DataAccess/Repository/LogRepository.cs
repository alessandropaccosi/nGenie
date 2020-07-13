using Newtonsoft.Json;
using nGenie.WebAnalysis.Application.DataAccess.Interfaces;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using nGenie.WebAnalysis.Application.Models;
using nGenie.WebAnalysis.Application.Models.DbModel;

namespace nGenie.WebAnalysis.Application.DataAccess.Repository
{
    public class LogRepository : BaseRepository, IDisposable
    {
        public LogRepository() : base()
        {
        }

        public static int Insert(Log log)
        {
            using (var repository = new LogRepository())
            {
                return repository.insert(log);
            }
        }

        private int insert(Log log)
        {
            Context.Log.Add(log);
            Context.SaveChanges();

            return log.Id;
        }
    }
}