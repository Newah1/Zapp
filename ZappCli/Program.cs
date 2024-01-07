using Cocona;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zapp.Models.PPL;
using Zapp.Services;
using ZappCli.Commands;

var builder = CoconaApp.CreateBuilder();
builder.Services.AddSingleton<IPPLService, PPLService>();

var configurationBuilder = new ConfigurationBuilder().AddEnvironmentVariables();
var config = configurationBuilder.Build();

if (!config.GetSection("PPLPassword").Exists())
{
    throw new Exception("No password");
}

var pplCredentials = new PPLCredentials()
{
    PPLPassword = config["PPLPassword"] ?? "",
    PPLUsername = config["PPLUsername"] ?? ""
};

builder.Services.AddSingleton(pplCredentials);
builder.Services.AddSingleton(config);

var app = builder.Build();

app.AddCommands<PPLCommands>();

app.Run();
