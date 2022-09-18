using AzureTableStorage.Service;

var builder = WebApplication.CreateBuilder(args);

// read more
// https://github.com/Azure/azure-sdk-for-net/blob/Azure.Data.Tables_12.6.1/sdk/tables/Azure.Data.Tables/README.md
// https://www.youtube.com/watch?v=HSL1poL1VR0

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
