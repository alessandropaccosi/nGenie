using Newtonsoft.Json;
using nGenie.WebAnalysis.Application.DataAccess.Interfaces;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using nGenie.WebAnalysis.Application.Models;
using nGenie.WebAnalysis.Application.Models.DbModel;
using nGenie.WebAnalysis.Application.Common;

namespace nGenie.WebAnalysis.Application.DataAccess.Repository
{
    public class CubiRepository : BaseRepository, IDisposable
    {
        public CubiRepository() : base()
        {
        }

        //public static void Test()
        //{
        //    using (var repository = new CubiRepository())
        //    {
        //        repository.test();
        //    }
        //}

        //private CategoriaFiltro[] test()
        //{
        //    var query =
        //            from
        //                cubo in Context.CategoriaFiltro
        //            //where

        //            select
        //                cubo;

        //    return query.ToArray();
        //}

        public static int InsertOrUpdate(CategoriaFiltro cubo)
        {
            using (var repository = new CubiRepository())
            {
                return repository.insertOrUpdate(cubo);
            }
        }

        public static int Update(CategoriaFiltro cubo)
        {
            using (var repository = new CubiRepository())
            {
                return repository.update(cubo);
            }
        }

        /// <summary>
        /// Restituisce informazioni sul Cubo corrispondente all'identificativo specificato. 
        /// Tenere presente che saranno restituiti dati sul cubo indipendentemente se il cubo e' Attivo o se riferisce
        /// Database e server attivi.
        /// </summary>
        /// <param name="cuboId"></param>
        /// <returns></returns>
        public static CategoriaFiltro Load(int cuboId)
        {
            FiltroRicercaCubo filtro = new FiltroRicercaCubo();
            filtro.CuboId = cuboId;

            CategoriaFiltro[] searchResult = Search(filtro);
            if (searchResult.Length == 0)
                throw new UserException(Costanti.MSG_CUBO_NON_ESISTENTE);

            return searchResult[0];
        }

        public static CategoriaFiltro[] Search(FiltroRicercaCubo filtro)
        {
            using (var repository = new CubiRepository())
            {
                return repository.search(filtro);
            }
        }

        //
        //Metodi privati
        //

        private int insertOrUpdate(CategoriaFiltro cubo)
        {
            return cubo.PK_CategoriaFiltroId <= 0 ?
                insert(cubo)
                : update(cubo);
        }

        private int insert(CategoriaFiltro cubo)
        {
            Context.CategoriaFiltro.Add(cubo);
            Context.SaveChanges();

            return cubo.PK_CategoriaFiltroId;
        }

        private int update(CategoriaFiltro cubo)
        {
            Context.Entry(cubo).State = EntityState.Modified;
            Context.SaveChanges();

            return cubo.PK_CategoriaFiltroId;
        }

        private CategoriaFiltro[] search()
        {
            var query =
                    from
                        cubo in Context.CategoriaFiltro
                    select
                        cubo;

            return query.ToArray();
        }

        private CategoriaFiltro[] search(FiltroRicercaCubo filtro)
        { 
            var query =
                    from
                        Cubo in Context.CategoriaFiltro
                            join Database in Context.DatabasesOlap
                                on Cubo.DatabaseOlapId equals Database.Id
                            join Server in Context.ServerOlap
                                 on Database.ServerOlapId equals Server.Id
                    where
                        (!filtro.CuboId.HasValue || Cubo.PK_CategoriaFiltroId == filtro.CuboId.Value)
                            &&
                        (string.IsNullOrEmpty(filtro.Nome) || Cubo.CubeName.ToLower() == filtro.Nome.ToLower())
                            &&
                        (!filtro.DatabaseId.HasValue || Cubo.DatabaseOlapId == filtro.DatabaseId)
                            &&
                        (!filtro.Attivo.HasValue || Cubo.Attivo == filtro.Attivo.Value)
                            &&
                        (!filtro.Attivo.HasValue || Database.Attivo == filtro.Attivo.Value)
                            &&
                        (!filtro.Attivo.HasValue || Server.Attivo == filtro.Attivo.Value)
                    select
                        Cubo;

            return query.ToArray();
        }
    }
}