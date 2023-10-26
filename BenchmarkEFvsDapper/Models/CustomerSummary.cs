namespace BenchmarkEFvsDapper.Models;

public class CustomerSummary
{
    public string CustomerName { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalOrderValue { get; set; }
}