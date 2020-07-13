
namespace nGenie.WebAnalysis.Application.Models
{
    using System.Data.Entity;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using nGenie.WebAnalysis.Application.DataAccess;

    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("TempCaringServiceEntities", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<nGenie.WebAnalysis.Application.Models.MyCategoriaFiltro> MyCategoriaFiltroes { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<User>()
        //        .HasMany(u => u.Roles)
        //        .WithMany(r => r.Users)
        //        .Map(m =>
        //        {
        //            m.ToTable("UserRoles");
        //            m.MapLeftKey("UserId");
        //            m.MapRightKey("RoleId");
        //        });
        //}

        //public DbSet<User> Users { get; set; }
        //public DbSet<Role> Roles { get; set; }
    }
}