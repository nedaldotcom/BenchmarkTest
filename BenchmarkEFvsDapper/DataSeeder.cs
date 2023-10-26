using BenchmarkEFvsDapper.DbContexts;
using BenchmarkEFvsDapper.DbContexts.Models;

namespace BenchmarkEFvsDapper;

public class DataSeeder
{
    private readonly CustomerOrdersDbContext _context;

    public DataSeeder(CustomerOrdersDbContext context)
    {
        _context = context;
    }

    public void SeedData()
    {
        if (!_context.Customers.Any())
        {
            for (int i = 1; i <= 1000000; i++)
            {
                var customer = new Customer { Name = $"Customer {i}" };
                _context.Customers.Add(customer);
            }
            _context.SaveChanges();
        }

        if (!_context.Orders.Any())
        {
            var random = new Random();
            for (int i = 1; i <= 1000000; i++)
            {
                var order = new Order { OrderDate = DateTime.Now.ToUniversalTime(), CustomerId = random.Next(1, 1000001) };
                _context.Orders.Add(order);
            }
            _context.SaveChanges();
        }

        if (!_context.OrderDetails.Any())
        {
            var random = new Random();
            for (int i = 1; i <= 1000000; i++)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = random.Next(1, 1000001),
                    ProductName = $"Product {i}",
                    Price = (decimal)(random.Next(1, 1000) + random.NextDouble()),
                    Quantity = random.Next(1, 11)
                };
                _context.OrderDetails.Add(orderDetail);
            }
            _context.SaveChanges();
        }
    }
}