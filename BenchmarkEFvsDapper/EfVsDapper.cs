using System.Data;
using BenchmarkDotNet.Attributes;
using BenchmarkEFvsDapper.DbContexts;
using BenchmarkEFvsDapper.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BenchmarkEFvsDapper;

public class EfVsDapper
{
    private readonly string _connectionString = "Data Source=.\\SQLEXPRESS;Database=MyDbContext;Trusted_Connection=true;TrustServerCertificate=True;";
    
    [Benchmark]
    public List<CustomerSummary> GetCustomerEfWithoutAsyncAwaitAsNoTracking()
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContext<CustomerOrdersDbContext>(options =>
                options.UseSqlServer(_connectionString))
            .BuildServiceProvider();

        using (var _dbContext = serviceProvider.GetRequiredService<CustomerOrdersDbContext>())
        {
            return _dbContext.Customers.AsNoTracking()
                .Join(_dbContext.Orders, c => c.Id, o => o.CustomerId, (c, o) => new { Customer = c, Order = o })
                .Join(_dbContext.OrderDetails, co => co.Order.Id, od => od.OrderId,
                    (co, od) => new { Customer = co.Customer, Order = co.Order, OrderDetail = od })
                .Where(co => co.Order.OrderDate.Year == 2023)
                .GroupBy(co => co.Customer.Name)
                .Select(g => new CustomerSummary
                {
                    CustomerName = g.Key,
                    TotalOrders = g.Count(),
                    TotalOrderValue = g.Sum(co => co.OrderDetail.Price)
                })
                .OrderByDescending(c => c.TotalOrders)
                .ToList();
        }
        
    }
    
    [Benchmark]
    public List<CustomerSummary> GetCustomerEfWithoutAsyncAwaitAndWithAsNoTracking()
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContext<CustomerOrdersDbContext>(options =>
                options.UseSqlServer(_connectionString))
            .BuildServiceProvider();

        using (var _dbContext = serviceProvider.GetRequiredService<CustomerOrdersDbContext>())
        {
            return _dbContext.Customers
                .Join(_dbContext.Orders, c => c.Id, o => o.CustomerId, (c, o) => new { Customer = c, Order = o })
                .Join(_dbContext.OrderDetails, co => co.Order.Id, od => od.OrderId,
                    (co, od) => new { Customer = co.Customer, Order = co.Order, OrderDetail = od })
                .Where(co => co.Order.OrderDate.Year == 2023)
                .GroupBy(co => co.Customer.Name)
                .Select(g => new CustomerSummary
                {
                    CustomerName = g.Key,
                    TotalOrders = g.Count(),
                    TotalOrderValue = g.Sum(co => co.OrderDetail.Price)
                })
                .OrderByDescending(c => c.TotalOrders)
                .ToList();
        }
        
    }
    
    [Benchmark]
    public async Task<List<CustomerSummary>> GetCustomerEfWithAsyncAwaitAndAsNoTracking()
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContext<CustomerOrdersDbContext>(options =>
                options.UseSqlServer(_connectionString))
            .BuildServiceProvider();

        using (var _dbContext = serviceProvider.GetRequiredService<CustomerOrdersDbContext>())
        {
            return await _dbContext.Customers.AsNoTracking()
                .Join(_dbContext.Orders, c => c.Id, o => o.CustomerId, (c, o) => new { Customer = c, Order = o })
                .Join(_dbContext.OrderDetails, co => co.Order.Id, od => od.OrderId,
                    (co, od) => new { Customer = co.Customer, Order = co.Order, OrderDetail = od })
                .Where(co => co.Order.OrderDate.Year == 2023)
                .GroupBy(co => co.Customer.Name)
                .Select(g => new CustomerSummary
                {
                    CustomerName = g.Key,
                    TotalOrders = g.Count(),
                    TotalOrderValue = g.Sum(co => co.OrderDetail.Price)
                })
                .OrderByDescending(c => c.TotalOrders)
                .ToListAsync();
        }
        
    }
    
    [Benchmark]
    public async Task<List<CustomerSummary>> GetCustomerEfWithoutAsNoTracking()
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContext<CustomerOrdersDbContext>(options =>
                options.UseSqlServer(_connectionString))
            .BuildServiceProvider();

        using (var _dbContext = serviceProvider.GetRequiredService<CustomerOrdersDbContext>())
        {
            return await _dbContext.Customers
                .Join(_dbContext.Orders, c => c.Id, o => o.CustomerId, (c, o) => new { Customer = c, Order = o })
                .Join(_dbContext.OrderDetails, co => co.Order.Id, od => od.OrderId,
                    (co, od) => new { Customer = co.Customer, Order = co.Order, OrderDetail = od })
                .Where(co => co.Order.OrderDate.Year == 2023)
                .GroupBy(co => co.Customer.Name)
                .Select(g => new CustomerSummary
                {
                    CustomerName = g.Key,
                    TotalOrders = g.Count(),
                    TotalOrderValue = g.Sum(co => co.OrderDetail.Price)
                })
                .OrderByDescending(c => c.TotalOrders)
                .ToListAsync();
        }
        
    }
    
    [Benchmark]
    public List<CustomerSummary> GetCustomerDapper()
    {
        using (IDbConnection dbConnection = new SqlConnection(_connectionString))
        {
            var sqlQuery = @"
            SELECT
                c.Name AS CustomerName,
                COUNT(o.Id) AS TotalOrders,
                SUM(od.Price) AS TotalOrderValue
            FROM Customers c
            INNER JOIN Orders o ON c.Id = o.CustomerId
            INNER JOIN OrderDetails od ON o.Id = od.OrderId
            WHERE YEAR(o.OrderDate) = 2023
            GROUP BY c.Name
            ORDER BY TotalOrders DESC";   
    
            // Execute the query and capture the results
            var results = dbConnection.Query<CustomerSummary>(sqlQuery).ToList();
    
            return results;
        }
    }
}