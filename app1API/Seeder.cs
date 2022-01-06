using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using app1API.Entities;
using Microsoft.EntityFrameworkCore;

namespace app1API
{
    public class Seeder
    {
        private readonly RestaurantDbContext _dbContext;

        public Seeder(RestaurantDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                var pendingMigrations = _dbContext.Database.GetPendingMigrations();

                if (pendingMigrations != null && pendingMigrations.Any())
                {
                    _dbContext.Database.Migrate();
                }
            }
        }
    }
}
