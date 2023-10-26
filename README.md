# Dapper vs Entity Framework Benchmark

A benchmark project that compares the performance of Dapper and Entity Framework for data access in a .NET application.

## Description

This benchmark project is designed to compare the performance of two popular data access technologies in .NET: Dapper and Entity Framework. It measures the speed and efficiency of data retrieval to help developers make informed decisions about which technology to use in their applications.

## Results

The measurements were conducted under various scenarios, including different data volumes.

### Benchmark Scenario 1: Small Data Set

For a small data set (e.g., 100 records), both Dapper and Entity Framework perform similarly with a negligible difference of approximately ±0.1 seconds in execution time. 

### Benchmark Scenario 2: Medium Data Set

When working with a medium-sized data set (e.g., 10,000 records), both Dapper and Entity Framework continue to exhibit a minimal difference in execution time, typically within the range of ±0.5 seconds.

### Benchmark Scenario 3: Large Data Set

With a large data set of one million records, the performance gap between Dapper and Entity Framework remains consistent, showing an execution time difference of approximately ±1 second. This demonstrates that both technologies maintain their relative performance levels even when dealing with substantial data volumes.

In summary, the benchmarks indicate that while there is a slight difference in execution time between Dapper and Entity Framework, the variance remains within a narrow range, For applications with extensive and complex database operations, Entity Framework's query generation and management capabilities make it a highly suitable choice for your data access needs.



## Customizing the Data Seeder

The `DataSeeder` file allows you to populate your database with sample data. To tailor this data to your specific requirements, follow these steps:

1. Open the `DataSeeder.cs` file in your project.
2. Locate the `SeedData` method, which is responsible for seeding data into the database.
3. Customize the seeding logic to match your use case. You can change the number of records, generate different data, or modify any other aspects of the seeding process.

**Example:**

```csharp
public void SeedData()
{
    // Customize the orders and order details seeding logic here.
    if (!_context.Customers.Any())
    {
        for (int i = 1; i <= 1000000; i++)
        {
            var customer = new Customer { Name = $"Customer {i}" };
            _context.Customers.Add(customer);
        }
        _context.SaveChanges();
    }

    // ...
}
```
