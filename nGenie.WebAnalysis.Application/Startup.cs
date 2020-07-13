using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Owin;

[assembly: OwinStartupAttribute(typeof(nGenie.WebAnalysis.Application.Startup))]
namespace nGenie.WebAnalysis.Application
{
    //using nGenie.Authentication.Windows.Middleware;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //Istruzione necessaria per usare la libreria signalR
            app.MapSignalR();

            //app.SetDefaultSignInAsAuthenticationType(WindowsAuthenticationDefaults.AuthenticationType);

            //app.UseWindowsAuthentication(new WindowsAuthenticationOptions()
            //{
            //    AuthenticationMode = AuthenticationMode.Active,
            //    SignInAsAuthenticationType = WindowsAuthenticationDefaults.AuthenticationType,
            //    AuthenticationType = WindowsAuthenticationDefaults.AuthenticationType,
            //    //LoginPath = new PathString("/Account/Login"),
            //    Provider = new WindowsAuthenticationProvider
            //    {
            //        OnAuthenticated = context =>
            //        {
            //            // context.Identity is of type ClaimsIdentity and can be extended;

            //            context.Identity.AddClaim(new Claim("app:name", "nGenie.WebAnalysis.Application"));

            //            return Task.FromResult(true);
            //        }
            //    }
            //});

            //Commentato per autenticazione windows
            //ConfigureAuth(app);
        }
    }
}
