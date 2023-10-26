using System.Data;
using BenchmarkEFvsDapper.DbContexts;
using BenchmarkEFvsDapper.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BenchmarkEFvsDapper.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class QueryController : ControllerBase
{
    private const string _connectionString =
        "Data Source=.\\SQLEXPRESS;Database=MyDbContext;Trusted_Connection=true;TrustServerCertificate=True;";

     [HttpGet]
     public CustomerResult GetCustomerDapper()
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

             var results = dbConnection.Query<CustomerSummary>(sqlQuery).ToList();

              return new CustomerResult
             {
                 CustomerSummaries = results,
                 ListLength = results.Count
             };
         }
     }
     
     [HttpGet]
     public async Task<CustomerResult> GetCustomerEf()
     {
         var serviceProvider = new ServiceCollection()
             .AddDbContext<CustomerOrdersDbContext>(options =>
                 options.UseSqlServer(_connectionString))
             .BuildServiceProvider();

         using (var _dbContext = serviceProvider.GetRequiredService<CustomerOrdersDbContext>())
         {
             var result = await _dbContext.Customers.AsNoTracking()
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

             return new CustomerResult()
             {
                 CustomerSummaries = result,
                 ListLength = result.Count
             };
         }
     }
}