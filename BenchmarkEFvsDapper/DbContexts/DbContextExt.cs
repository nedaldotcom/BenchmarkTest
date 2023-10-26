using Microsoft.EntityFrameworkCore;

namespace BenchmarkEFvsDapper.DbContexts;

public static class DbContextExt
{
    public static IServiceCollection AddCustomerOrdersDbContext(this IServiceCollection services)
    {
        using (var scope = services.BuildServiceProvider())
        {
            var configuration = scope.GetService<IConfiguration>();
            var connectionString = configuration?.GetConnectionString("CustomerOrdersDbContext");

            services.AddDbContext<CustomerOrdersDbContext>(options =>
            {
                options.UseSqlServer(connectionString,
                    x => x.MigrationsHistoryTable("__MigrationsHistoryForMyDbContext", "migrations"));
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            });
        }

        using (var scope = services.BuildServiceProvider())
        {
            var customerOrdersDb = scope.GetService<CustomerOrdersDbContext>();
            customerOrdersDb?.Database?.Migrate();
            var seeder = new DataSeeder(customerOrdersDb);
            seeder.SeedData();
        }

        return services;
    }
}