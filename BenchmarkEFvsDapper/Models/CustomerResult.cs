namespace BenchmarkEFvsDapper.Models;

public class CustomerResult
{
    public List<CustomerSummary> CustomerSummaries { get; set; }
    public int ListLength { get; set; }
}