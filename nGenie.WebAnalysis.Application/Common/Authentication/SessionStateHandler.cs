
namespace nGenie.WebAnalysis.Application.Common.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.SessionState;

    public class SessionStateHandler : IHttpHandler, IReadOnlySessionState
    {
        public bool IsReusable { get { return true; } }

        public void ProcessRequest(HttpContext ctx)
        {
            ctx.Response.Write(ctx.Session["NGENIE"]);
        }
    }
}