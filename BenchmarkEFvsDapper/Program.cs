using BenchmarkDotNet.Running;
using BenchmarkEFvsDapper;
using BenchmarkEFvsDapper.DbContexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCustomerOrdersDbContext();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

BenchmarkRunner.Run<EfVsDapper>();

app.Run();