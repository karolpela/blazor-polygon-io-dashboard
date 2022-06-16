using Duende.IdentityServer.EntityFramework.Options;
using FinanceApp.Server.Models;
using FinanceApp.Server.Models.TickerDetails;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Results = FinanceApp.Server.Models.TickerDetails.Results;

namespace FinanceApp.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public DbSet<TickerDetails> TickerDetails { get; set; } = null!;

        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TickerDetails>(e =>
            {
                e.HasKey(td => td.TickerId);
                e.ToTable("TickerDetails");

                e.HasMany(td => td.ApplicationUsers)
                    .WithMany(td => td.TickerWatchlist)
                    .UsingEntity(j => j.ToTable("TickerDetailsApplicationUser"));
            });

            builder.Entity<Address>(e =>
            {
                e.HasKey(a => a.Address1);
            });

            builder.Entity<Branding>(e =>
            {
                e.HasKey(b => b.LogoUrl);
            });

            builder.Entity<Results>(e =>
            {
                e.HasKey(r => r.Ticker);
            });
        }
    }
}