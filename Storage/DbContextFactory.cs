using EntityFramework.Commands;
using EntityFramework.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EntityFramework
{
    public static class DbContextFactoryExtensions
    {
        private static readonly Assembly _efAssembly = typeof(DbContextFactoryExtensions).Assembly;

        public static IServiceCollection AddEntityFramework(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PostgreSQLConnection")));

            _efAssembly.Register(services, typeof(QueriesBase<>));
            _efAssembly.Register(services, typeof(CommandsBase<>));

            return services;
        }

        public static void Register(this Assembly assembly,
                                    IServiceCollection services,
                                    Type baseType)
        {
            var types = assembly.ExportedTypes
               .Where(x => x.IsClass && x.IsPublic && !x.IsAbstract && x.BaseType.IsGenericType && x.BaseType.GetGenericTypeDefinition().IsAssignableFrom(baseType));

            foreach (var type in types)
            {
                services.AddScoped(type);
            }
        }
    }
}
