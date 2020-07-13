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
    public class ReportRepository: BaseRepository, IDisposable
    {
        public ReportRepository() : base()
        {
        }

        public static int Insert(FiltroUtente report, int[] condividiConUtenti)
        {
            using (var context = new ReportRepository())
            {
                return context.insert(report, condividiConUtenti);
            }
        }

        public static void Update(FiltroUtente report, int[] condividiConUtenti)
        {
            using (var context = new ReportRepository())
            {
                context.update(report, condividiConUtenti);
            }
        }

        public static bool Exists(FiltroRicercaReport filtro)
        {
            FiltroUtente[] searchResult = Search(filtro);
            return searchResult.Length > 0;
        }

        public static FiltroUtente[] Search(FiltroRicercaReport filtro)
        {
            using (var context = new ReportRepository())
            {
                return context.search(filtro);
            }
        }

        public static FiltroUtente Load(int reportId)
        {
            FiltroRicercaReport filtro = new FiltroRicercaReport();
            filtro.Id = reportId;
            FiltroUtente[] result = Search(filtro);
            if (result.Length == 0)
                throw new UserException("Report non trovato");

            return result[0];
        }

        /// <summary>
        /// Permette di attivare o disattivare il report predefinito per l'utente e il cubo specificato
        /// </summary>
        /// <param name="utenteId"></param>
        /// <param name="cuboId"></param>
        /// <param name="attiva">Riportare true per attivare il Report, false per disattivarlo</param>
        //public static void AttivaDisattivaReportPredefinito(int utenteId, int cuboId, bool attiva)
        //{
        //    using (var context = new ReportRepository())
        //    {
        //        context.attivaDisattivaReportPredefinito(utenteId, cuboId, attiva);
        //    }
        //}

        private int insert(FiltroUtente report, int[] condividiConUtenti)
        {
            int result;
            Context.FiltroUtente.Add(report);
            Context.SaveChanges();
            result = report.PKFiltroId;
            contextCondividiReport(report, condividiConUtenti);

            return result;
        }

        private void update(FiltroUtente report, int[] condividiConUtenti)
        {
            Context.Entry(report).State = EntityState.Modified;
            Context.SaveChanges();

            contextCondividiReport(report, condividiConUtenti);
        }

        private void contextCondividiReport(FiltroUtente report, int[] condividiConUtenti)
        {
            if (condividiConUtenti.Length > 0)
            {
                string nomeOriginale = report.Nome;
                //int reportIdOriginale = report.PKFiltroId;

                //Aggiunge un nuovo Report simile a quello originale
                report.Nome = Guid.NewGuid().ToString();
                report.Condiviso = true;
                Context.FiltroUtente.Add(report);
                Context.SaveChanges();
                int newReportId = report.PKFiltroId;

                //Aggiunge un nuovo Report condiviso 
                ReportCondiviso reportCondiviso = new ReportCondiviso();
                reportCondiviso.Nome = nomeOriginale;
                reportCondiviso.ReportId = newReportId;
                Context.ReportCondivisi.Add(reportCondiviso);
                Context.SaveChanges();

                //Inserisce gli utenti destinatari del Report condiviso
                foreach (int utenteId in condividiConUtenti)
                {
                    UtenteReportCondiviso utenteReport = new UtenteReportCondiviso();
                    utenteReport.ReportCondivisoId = reportCondiviso.Id;
                    utenteReport.DestinatarioUtenteId = utenteId;
                    Context.UtentiReportCondivisi.Add(utenteReport);
                }
                Context.SaveChanges();
            }
        }

        private FiltroUtente[] search(FiltroRicercaReport filtro)
        {
            var result = Context.FiltroUtente.Where(e =>
                (!filtro.Id.HasValue || e.PKFiltroId == filtro.Id)
                    &&
                (filtro.Nome == null || e.Nome == filtro.Nome)
                    &&
                (!filtro.UtenteId.HasValue || e.FK_UtenteId == filtro.UtenteId)
                    &&
                (!filtro.CuboId.HasValue || e.FK_CategoriaId == filtro.CuboId)
                    && 
                (!filtro.Attivo.HasValue || e.Attivo == filtro.Attivo)
                    &&
                (!filtro.Predefinito.HasValue || e.Predefinito == filtro.Predefinito)
            );

            return result.ToArray();
        }

        //private void attivaDisattivaReportPredefinito(int utenteId, int cuboId, bool attiva)
        //{
        //    //Imposta un filtro di ricerca per individuare il Report predefinito interessato
        //    FiltroRicercaReport filtroRicerca = new FiltroRicercaReport();
        //    filtroRicerca.Predefinito = true;
        //    filtroRicerca.CuboId = cuboId;
        //    filtroRicerca.UtenteId = utenteId;

        //    FiltroUtente report= search(filtroRicerca).FirstOrDefault();
        //    if (report != null)
        //    {
        //        report.Attivo = attiva; 
        //        update(report);
        //    }
        //}

        //private bool disposed = false;

        //private TempCaringServiceEntities _context;

        //public ReportRepository(TempCaringServiceEntities context)
        //{
        //    this._context = context;
        //}

        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!this.disposed)
        //    {
        //        if (disposing)
        //        {
        //            _context.Dispose();
        //        }
        //    }
        //    this.disposed = true;
        //}
    }
}