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
    public class ServerOlapRepository : BaseRepository, IDisposable
    {
        public ServerOlapRepository() : base()
        {
        }

        public static int InsertOrUpdate(ServerOlap serverOlap)
        {
            using (var repository = new ServerOlapRepository())
            {
                return repository.insertOrUpdate(serverOlap);
            }
        }

        public static int Update(ServerOlap serverOlap)
        {
            using (var repository = new ServerOlapRepository())
            {
                return repository.update(serverOlap);
            }
        }


        public static ServerOlap Load(int id)
        {
            FiltroRicercaServer filtro = new FiltroRicercaServer();
            filtro.ServerOlapId = id;

            ServerOlap server = Search(filtro).FirstOrDefault();

            if (server == null)
                throw new UserException("Server non esistente");

            return server;
        }

        /// <summary>
        /// Restituisce tutti i server
        /// </summary>
        /// <returns></returns>
        public static ServerOlap[] Load()
        {
            return Search(new FiltroRicercaServer());
        }

        ///// <summary>
        ///// Restituisce tutti i server attivi
        ///// </summary>
        ///// <returns></returns>
        //public static ServerOlap[] Search(bool attivo)
        //{
        //    FiltroRicercaServer filtro = new FiltroRicercaServer { Attivo = attivo };
        //    return Search(filtro);
        //}

        public static ServerOlap[] Search(FiltroRicercaServer filtro)
        {

            using (var repository = new ServerOlapRepository())
            {
                return repository.search(filtro);
            }
        }

        private int insertOrUpdate(ServerOlap database)
        {
            return database.Id <= 0 ?
                insert(database)
                : update(database);
        }

        private int insert(ServerOlap database)
        {
            if (databaseExists(database.Nome))
                throw new UserException("Server già esistente");

            Context.ServerOlap.Add(database);
            Context.SaveChanges();

            return database.Id;
        }

        private int update(ServerOlap database)
        {
            Context.Entry(database).State = EntityState.Modified;
            Context.SaveChanges();

            return database.Id;
        }

        private bool databaseExists(string nome)
        {
            FiltroRicercaServer filtro = new FiltroRicercaServer();
            filtro.Nome = nome;

            return search(filtro).Length > 0;
        }

        private ServerOlap[] search(FiltroRicercaServer filtro)
        {
            var query =
                    from
                        server in Context.ServerOlap.Include("DatabasesOlap")
                    where
                        (string.IsNullOrEmpty(filtro.Nome) || server.Nome.ToLower() == filtro.Nome.ToLower())
                            &&
                        (!filtro.ServerOlapId.HasValue || server.Id == filtro.ServerOlapId)
                            &&
                        (!filtro.Attivo.HasValue || server.Attivo == filtro.Attivo.Value)
                    select
                        server;

            return query.ToArray();
        }

        //private ServerOlap[] search2(FiltroRicercaServer filtro)
        //{
        //    var query =
        //            from
        //                cubi in Context.CategoriaFiltro
        //                    join databases in Context.DatabasesOlap on cubi.DatabaseOlapId equals databases.Id
        //                    join servers in Context.ServerOlap on databases.ServerOlapId equals servers.Id
        //            select
        //                servers;
        //            //    server in Context.ServerOlap.Include("DatabasesOlap")
        //            //where
        //            //    (string.IsNullOrEmpty(filtro.Nome) || server.Nome.Contains(filtro.Nome))
        //            //        &&
        //            //    (!filtro.ServerOlapId.HasValue || server.Id == filtro.ServerOlapId)
        //            //        &&
        //            //    (!filtro.Attivo.HasValue || server.Attivo == filtro.Attivo.Value)
        //            //select
        //            //    server;

        //    return query.ToArray();
        //}
    }
}