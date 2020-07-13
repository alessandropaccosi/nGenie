using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using nGenie.WebAnalysis.Application.Models;

namespace nGenie.WebAnalysis.Application.DataAccess.Interfaces
{
    public interface IFiltroUtenteRepository : IDisposable
    {
        int Insert(FiltroUtente report);
        //void Update(FiltroUtente report);
        //bool TryFind(FiltroUtente nomeReport, out int reportId);
        //bool Exists(FiltroUtente filtro);
    }
}