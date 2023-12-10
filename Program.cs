using Zapp.Models.PPL;
using Zapp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var configurationBuilder = new ConfigurationBuilder().AddEnvironmentVariables();
var config = configurationBuilder.Build();

if (!config.GetSection("PPLPassword").Exists())
{
    throw new Exception("No password");
}

var pplCredentials = new PPLCredentials()
{
    PPLPassword = config["PPLPassword"],
    PPLUsername = config["PPLUsername"]
};
Console.WriteLine(pplCredentials.PPLUsername);
builder.Services.AddSingleton(pplCredentials);
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(config);

// services
builder.Services.AddSingleton<IPPLService, PPLService>();

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
