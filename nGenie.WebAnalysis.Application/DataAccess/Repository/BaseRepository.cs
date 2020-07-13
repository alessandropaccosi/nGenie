using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using nGenie.WebAnalysis.Application.Models;

namespace nGenie.WebAnalysis.Application.DataAccess.Repository
{
    public class BaseRepository: IDisposable
    {
        protected TempCaringServiceEntities Context;

        public BaseRepository()
        {
            Context = new TempCaringServiceEntities();
        }

        public void Dispose()
        {
            if (Context != null)
            { 
                Context.Dispose();
                Context = null;
            }
        }
    }
}