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
    public class DatabasesOlapRepository : BaseRepository, IDisposable
    {
        public DatabasesOlapRepository() : base()
        {
        }

        public static int InsertOrUpdate(DatabaseOlap databaseOlap)
        {
            using (var repository = new DatabasesOlapRepository())
            {
                return repository.insertOrUpdate(databaseOlap);
            }
        }

        public static int Update(DatabaseOlap databaseOlap)
        {
            using (var repository = new DatabasesOlapRepository())
            {
                return repository.update(databaseOlap);
            }
        }


        public static DatabaseOlap Load(int id)
        {
            FiltroRicercaDatabase filtro = new FiltroRicercaDatabase();
            filtro.DatabaseOlapId = id;

            DatabaseOlap database = Search(filtro).FirstOrDefault();

            if (database == null)
                throw new UserException("Database non esistente");

            return database;
        }

        /// <summary>
        /// Restituisce tutti i database
        /// </summary>
        /// <returns></returns>
        public static DatabaseOlap[] Load()
        {
            return Search(new FiltroRicercaDatabase());
        }

        public static DatabaseOlap[] Search(FiltroRicercaDatabase filtro)
        {
            
            using (var repository = new DatabasesOlapRepository())
            {
                return repository.search(filtro);
            }
        }

        private int insertOrUpdate(DatabaseOlap database)
        {
            return database.Id <= 0 ?
                insert(database)
                : update(database);
        }

        private int insert(DatabaseOlap database)
        {
            if (databaseExists(database.Nome))
                throw new UserException("Database già esistente");

            Context.DatabasesOlap.Add(database);
            Context.SaveChanges();

            return database.Id;
        }

        private int update(DatabaseOlap database)
        {
            Context.Entry(database).State = EntityState.Modified;
            Context.SaveChanges();

            return database.Id;
        }

        private bool databaseExists(string nome)
        {
            FiltroRicercaDatabase filtro = new FiltroRicercaDatabase();
            filtro.Nome = nome;

            return search(filtro).Length > 0;
        }

        private DatabaseOlap[] search(FiltroRicercaDatabase filtro)
        {
            var query =
                    from
                        item in Context.DatabasesOlap.Include("ServerOlap")
                    where
                        (string.IsNullOrEmpty(filtro.Nome) || item.Nome.ToLower() == filtro.Nome.ToLower())
                            &&
                        (!filtro.DatabaseOlapId.HasValue || item.Id == filtro.DatabaseOlapId)
                            &&
                        (!filtro.Attivo.HasValue || item.Attivo == filtro.Attivo.Value)
                    select
                        item;

            return query.ToArray();
        }
    }
}