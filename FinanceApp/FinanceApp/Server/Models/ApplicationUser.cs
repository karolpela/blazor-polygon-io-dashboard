using Microsoft.AspNetCore.Identity;

namespace FinanceApp.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<TickerDetails.TickerDetails> TickerWatchlist { get; set; }

        public ApplicationUser()
        {
            TickerWatchlist = new HashSet<TickerDetails.TickerDetails>();
        }
    }
}