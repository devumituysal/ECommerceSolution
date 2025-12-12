using App.Data.Contexts;
using App.Data.Repositories.Abstractions;
using App.Data.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Repositories.Extensions
{
    public static class DataExtensions
    {
        public static void AddData(this IServiceCollection services,string connectionString)
        {
            services.AddDbContext<DbContext, AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IDataRepository, DataRepository>();
        }

    }
}
