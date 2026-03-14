using FXExchange.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
