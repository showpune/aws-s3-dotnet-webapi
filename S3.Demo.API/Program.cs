using Azure.Storage.Blobs;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Azure Blob Storage
string endpoint = builder.Configuration.GetValue<string>("AzureStorageBlob:Endpoint") 
    ?? throw new InvalidOperationException("AzureStorageBlob:Endpoint configuration is required");
builder.Services.AddSingleton(x => new BlobServiceClient(
    new Uri(endpoint),
    new DefaultAzureCredential()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
