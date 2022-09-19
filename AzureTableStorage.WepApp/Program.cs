using AzureTableStorage.Service;

var builder = WebApplication.CreateBuilder(args);

// read more
// https://github.com/Azure/azure-sdk-for-net/blob/Azure.Data.Tables_12.6.1/sdk/tables/Azure.Data.Tables/README.md
// https://www.youtube.com/watch?v=HSL1poL1VR0
// https://microsoft.github.io/AzureTipsAndTricks/blog/tip83.html

// read more for azurite
// https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=docker-hub
// https://blog.johnnyreilly.com/2021/05/15/azurite-and-table-storage-dev-container
// https://medium.com/cloudfordummies/local-azure-storage-emulation-with-azurite-and-azure-functions-a-dummies-guide-53949f0c1f44
// https://github.com/Azure/Azurite#introduction
// https://hub.docker.com/_/microsoft-azure-storage-azurite

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICustomerService, CustomerService>();

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
