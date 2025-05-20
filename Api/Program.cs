using IskiOpcSdk.Extensions;

var builder = WebApplication.CreateBuilder(args);

// OPC UA servis entegrasyonu
builder.Services.AddOpcUaClient();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Controllers
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
