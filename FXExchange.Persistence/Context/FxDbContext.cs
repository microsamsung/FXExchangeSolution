using FXExchange.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FXExchange.Persistence.Context
{
    public class FxDbContext : DbContext
    {
        public DbSet<ExchangeRate> Rates { get; set; }

        public FxDbContext(
            DbContextOptions<FxDbContext> options)
            : base(options)
        {
        }
    }
}
