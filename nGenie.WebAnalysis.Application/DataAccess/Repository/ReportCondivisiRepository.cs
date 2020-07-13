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
    public class ReportCondivisiRepository : BaseRepository, IDisposable
    {
        public ReportCondivisiRepository() : base()
        {
        }

        /// <summary>
        /// Restituisce informazioni sui Report condivisi visibilil all'utente specificato
        /// </summary>
        /// <param name="utenteId"></param>
        /// <returns></returns>
        public static ReportRicevutoInfo[] SearchReportRicevuti(int utenteId)
        {
            using (var repository = new ReportCondivisiRepository())
            {
                return repository.searchReportRicevuti(utenteId);
            }
        }

        public static ReportInviatoInfo[] SearchReportInviati(int utenteId)
        {
            using (var repository = new ReportCondivisiRepository())
            {
                return repository.searchReportInviati(utenteId);
            }
        }

        public static ReportCondiviso GetReportCondivisoByIdReport(int reportId)
        {
            using (var repository = new ReportCondivisiRepository())
            {
                return repository.getReportCondivisoByIdReport(reportId);
            }
        }

        private ReportRicevutoInfo[] searchReportRicevuti(int utenteId)
        {
            var query =
            (
                    from
                        UtentiReportCondivisi in Context.UtentiReportCondivisi
                    join ReportCondivisi in Context.ReportCondivisi
                        on UtentiReportCondivisi.ReportCondivisoId equals ReportCondivisi.Id
                    join Reports in Context.FiltroUtente
                        on ReportCondivisi.ReportId equals Reports.PKFiltroId
                    join Cubi in Context.CategoriaFiltro
                        on Reports.FK_CategoriaId equals Cubi.PK_CategoriaFiltroId
                    join Databases in Context.DatabasesOlap
                        on Cubi.DatabaseOlapId equals Databases.Id
                    join Servers in Context.ServerOlap
                        on Databases.ServerOlapId equals Servers.Id
                    join Utenti in Context.Utente
                        on UtentiReportCondivisi.DestinatarioUtenteId equals Utenti.PkUtenteId
                    join Mittenti in Context.Utente
                        on Reports.FK_UtenteId equals Mittenti.PkUtenteId
                    where
                        UtentiReportCondivisi.DestinatarioUtenteId == utenteId
                    orderby
                        Reports.PKFiltroId descending
                    select
                        new ReportRicevutoInfo
                        {
                            Mittente = Mittenti,
                            Destinatario = Utenti,
                            ReportCondiviso = ReportCondivisi,
                            Report = Reports,
                            Cubo = Cubi,
                            ServerOlap = Servers
                        }
            );

            return query.ToArray();
        }

        private ReportInviatoInfo[] searchReportInviati(int utenteId)
        {
            var query =
            (
                    from
                        ReportCondivisi in Context.ReportCondivisi
                            join Reports in Context.FiltroUtente
                                on ReportCondivisi.ReportId equals Reports.PKFiltroId
                            join Cubi in Context.CategoriaFiltro
                                on Reports.FK_CategoriaId equals Cubi.PK_CategoriaFiltroId
                            join Databases in Context.DatabasesOlap
                                on Cubi.DatabaseOlapId equals Databases.Id
                            join Servers in Context.ServerOlap
                                on Databases.ServerOlapId equals Servers.Id
                            //join Utenti in Context.Utente
                            //    on UtentiReportCondivisi.DestinatarioUtenteId equals Utenti.PkUtenteId
                            join Mittenti in Context.Utente
                                on Reports.FK_UtenteId equals Mittenti.PkUtenteId
                    where
                        Reports.FK_UtenteId == utenteId
                    orderby
                        Reports.PKFiltroId descending

                    select
                        new ReportInviatoInfo
                        {
                            Mittente = Mittenti,
                            //Utente = Utenti,
                            ReportCondiviso = ReportCondivisi,
                            Report = Reports,
                            Cubo = Cubi,
                            ServerOlap = Servers
                        }
            );

            return query.ToArray();
        }

        private ReportCondiviso getReportCondivisoByIdReport(int reportId)
        {
            var query =
            (
                    from
                        ReportCondivisi in Context.ReportCondivisi
                    where
                        ReportCondivisi.ReportId == reportId
                    select
                        ReportCondivisi
            );

            ReportCondiviso result = query.FirstOrDefault();
            if (result == null)
                throw new UserException("Report condiviso non trovato");

            return result;
        }
    }
}