using CommonLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SQLDatabase
{
    public static class DatabaseServices
    {
        public static void AddDatabaseServices(this IServiceCollection services)
        {
            services.AddSingleton<IDBCaller, DBCaller>();
        }
    }
}
