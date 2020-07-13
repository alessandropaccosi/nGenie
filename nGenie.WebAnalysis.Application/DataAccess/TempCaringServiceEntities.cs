//@@@@Cambiare namespace
//namespace nGenie.WebAnalysis.Application.DataAccess
using nGenie.WebAnalysis.Application.Models.DbModel;
namespace nGenie.WebAnalysis.Application
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Configuration;

    public partial class TempCaringServiceEntities : DbContext
    {
        public TempCaringServiceEntities()
            : base(ConfigurationManager.ConnectionStrings["TempCaringServiceEntities"].ConnectionString)
        {
             
            //@@@@Impedisce l'eventuale creazione del database
            Database.SetInitializer<TempCaringServiceEntities>(null);
        }

        //protected override void OnModelCreating
        //    (DbModelBuilder modelBuilder)
        //{
        //}

        public DbSet<CategoriaFiltro> CategoriaFiltro { get; set; }
        public DbSet<Utente> Utente { get; set; }
        public DbSet<FiltroUtente> FiltroUtente { get; set; }
        public DbSet<DatabaseOlap> DatabasesOlap { get; set; }

        public DbSet<ServerOlap> ServerOlap { get; set; }
        public DbSet<AutorizzazioneCubo> AutorizzazioniCubi { get; set; }

        public DbSet<Ruolo> Ruoli { get; set; }

        public DbSet<Log> Log { get; set; }

        public DbSet<ReportCondiviso> ReportCondivisi { get; set; }

        public DbSet<UtenteReportCondiviso> UtentiReportCondivisi { get; set; }
    }
}