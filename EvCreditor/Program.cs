using EvCreditor;
using EvCreditor.Adapters.Billit;
using EvCreditor.Adapters.Zaptec;

var builder = Host.CreateApplicationBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddZaptecSettings(builder.Configuration);
builder.Services.AddBillit(builder.Configuration);

builder.Services.AddZaptecServices();

builder.Services.AddHostedService<GetMonthlyChargingUsageJob>();

var host = builder.Build();
host.Run();
