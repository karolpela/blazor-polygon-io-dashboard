using FinanceApp.Server.Models.TickerDetails;
using Microsoft.AspNetCore.Identity;

namespace FinanceApp.Server.Models
{
    public sealed class ApplicationUser : IdentityUser
    {
        public ICollection<TickerResults> TickerWatchlist { get; set; }

        public ApplicationUser()
        {
            TickerWatchlist = new HashSet<TickerResults>();
        }
    }
}