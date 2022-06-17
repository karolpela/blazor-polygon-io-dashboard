using Duende.IdentityServer.EntityFramework.Options;
using FinanceApp.Server.Models;
using FinanceApp.Server.Models.TickerDetails;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FinanceApp.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public DbSet<TickerResults> Results => Set<TickerResults>();

        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Address>(e => { e.HasKey(a => a.Address1); });

            builder.Entity<Branding>(e => { e.HasKey(b => b.LogoUrl); });

            builder.Entity<TickerResults>(e =>
            {
                e.HasKey(tr => tr.Ticker);
                e.Property(tr => tr.Ticker).HasMaxLength(4);
                e.ToTable("TickerResults");

                e.HasMany(td => td.ApplicationUsers)
                    .WithMany(td => td.TickerWatchlist)
                    .UsingEntity(j => j.ToTable("ApplicationUserTickerResults"));
            });
        }
    }
}